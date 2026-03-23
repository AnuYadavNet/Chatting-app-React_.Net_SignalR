using ChattingApp.Application.DTOs;
using ChattingApp.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace ChattingApp.API.Hubs
{
    /// <summary>
    /// SignalR Hub — the real-time backbone of the application.
    ///
    /// DESIGN DECISIONS:
    /// - Uses Groups to route messages between specific users.
    ///   When User A and User B start chatting, they join a shared group named by sorting their IDs (e.g., "chat_UserA_UserB").
    ///   This ensures messages go only to the intended pair.
    ///
    /// - Hub methods are thin: they validate input, delegate to IChatService,
    ///   then broadcast. No business logic lives here.
    ///
    /// - Connection tracking: a static ConcurrentDictionary maps
    ///   ConnectionId → UserId for presence awareness.
    /// </summary>
    [Authorize]
    public class ChatHub : Hub
    {
        // Tracks active connections: ConnectionId → UserId
        // ConcurrentDictionary is thread-safe for concurrent SignalR connections
        private static readonly System.Collections.Concurrent.ConcurrentDictionary<string, string>
            _connectedUsers = new();

        private readonly IChatService _chatService;
        private readonly ILogger<ChatHub> _logger;

        public ChatHub(IChatService chatService, ILogger<ChatHub> logger)
        {
            _chatService = chatService;
            _logger = logger;
        }

        // -----------------------------------------------------------------------
        // Lifecycle
        // -----------------------------------------------------------------------

        /// <summary>
        /// Called automatically when a client connects.
        /// The client must call JoinChat() after connecting to register their identity.
        /// </summary>
        public override async Task OnConnectedAsync()
        {
            _logger.LogInformation("Client connected: ConnectionId={ConnectionId}", Context.ConnectionId);
            await base.OnConnectedAsync();
        }

        /// <summary>
        /// Called automatically when a client disconnects (tab close, network drop, etc.).
        /// Cleans up connection tracking and notifies the other user.
        /// </summary>
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            if (_connectedUsers.TryRemove(Context.ConnectionId, out var userId))
            {
                _logger.LogInformation(
                    "User disconnected: UserId={UserId}, ConnectionId={ConnectionId}",
                    userId, Context.ConnectionId);

                // Notify all clients that this user went offline
                await Clients.All.SendAsync("UserDisconnected", userId);
            }

            await base.OnDisconnectedAsync(exception);
        }

        // -----------------------------------------------------------------------
        // Hub Methods (called by clients)
        // -----------------------------------------------------------------------

        /// <summary>
        /// Registers the user's identity against this connection
        /// and places both users into a shared chat group.
        ///
        /// Client call:  connection.invoke("JoinChat", senderId, receiverId)
        /// </summary>
        public async Task JoinChat(string senderId, string receiverId)
        {
            if (string.IsNullOrWhiteSpace(senderId) || string.IsNullOrWhiteSpace(receiverId))
            {
                await Clients.Caller.SendAsync("Error", "Both senderId and receiverId are required.");
                return;
            }

            // Register this connection → userId mapping
            _connectedUsers[Context.ConnectionId] = senderId;

            // Build a deterministic, order-independent group name
            var groupName = GetGroupName(senderId, receiverId);
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

            _logger.LogInformation(
                "User joined chat group: UserId={UserId}, Group={Group}",
                senderId, groupName);

            // Notify the caller that they've successfully joined
            await Clients.Caller.SendAsync("JoinedChat", groupName);

            // Notify the group (including the other user if already connected)
            await Clients.Group(groupName).SendAsync("UserConnected", senderId);
        }

        /// <summary>
        /// Sends a message from one user to another.
        /// Flow:
        ///   1. Client invokes SendMessage(dto)
        ///   2. Hub delegates to ChatService (saves to DB)
        ///   3. Hub broadcasts the saved MessageDto to the chat group
        /// Client call:  connection.invoke("SendMessage", { senderId, receiverId, messageText })
        /// </summary>
        public async Task SendMessage(SendMessageDto sendMessageDto)
        {
            try
            {
                // Delegate to the service layer — hub stays thin
                var savedMessage = await _chatService.SendMessageAsync(sendMessageDto);

                var groupName = GetGroupName(sendMessageDto.SenderId, sendMessageDto.ReceiverId);

                // Broadcast the fully hydrated message (with MessageId + Timestamp) to the group
                await Clients.Group(groupName).SendAsync("ReceiveMessage", savedMessage);

                _logger.LogInformation(
                    "Message broadcast to group {Group}: MessageId={MessageId}",
                    groupName, savedMessage.MessageId);
            }
            catch (ArgumentException ex)
            {
                await Clients.Caller.SendAsync("Error", ex.Message);
            }
            catch (Exception ex) 
            {
                _logger.LogError(ex, "Unexpected error in SendMessage hub method.");
                await Clients.Caller.SendAsync("Error", "An unexpected error occurred. Please try again.");
            }
        }

        /// <summary>
        /// Loads and returns the chat history between two users.
        /// Called once when the UI mounts, so both panels are pre-populated.
        ///
        /// Client call:  connection.invoke("LoadChatHistory", userA, userB)
        /// </summary>
        public async Task LoadChatHistory(string userA, string userB)
        {
            try
            {
                var history = await _chatService.GetChatHistoryAsync(userA, userB);

                // Send only to the requesting connection, not the whole group
                await Clients.Caller.SendAsync("ChatHistoryLoaded", history);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading chat history for {UserA}/{UserB}", userA, userB);
                await Clients.Caller.SendAsync("Error", "Failed to load chat history.");
            }
        }

        // -----------------------------------------------------------------------
        // Helpers
        // -----------------------------------------------------------------------

        /// <summary>
        /// Creates a consistent group name regardless of which user initiates.
        /// Sorting ensures "Alice_Bob" and "Bob_Alice" resolve to the same group.
        /// </summary>
        private static string GetGroupName(string userA, string userB)
        {
            var sorted = new[] { userA.ToLower(), userB.ToLower() };
            Array.Sort(sorted);
            return $"chat_{sorted[0]}_{sorted[1]}";
        }
    }
}
