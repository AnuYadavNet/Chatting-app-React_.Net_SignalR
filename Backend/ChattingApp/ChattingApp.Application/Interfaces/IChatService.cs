using ChattingApp.Application.DTOs;

namespace ChattingApp.Application.Interfaces
{
    /// <summary>
    /// Defines the business-logic contract for chat operations.
    /// Controllers and Hubs depend on this interface, not the concrete service.
    /// </summary>
    public interface IChatService
    {
        /// <summary>
        /// Validates, maps, and persists a new message.
        /// Returns the saved MessageDto (with assigned MessageId and Timestamp).
        /// </summary>
        Task<MessageDto> SendMessageAsync(SendMessageDto sendMessageDto);

        /// <summary>
        /// Returns paginated chat history between two users.
        /// Messages are sorted oldest-first for natural chat display.
        /// </summary>
        Task<IEnumerable<MessageDto>> GetChatHistoryAsync(string userA, string userB);

        /// <summary>
        /// Triggers deletion of messages older than 12 hours.
        /// Invoked by the IHostedService background worker.
        /// </summary>
        Task CleanupOldMessagesAsync();
    }
}
