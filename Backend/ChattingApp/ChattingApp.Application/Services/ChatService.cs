using ChattingApp.Application.DTOs;
using ChattingApp.Application.Interfaces;
using ChattingApp.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace ChattingApp.Application.Services
{
    /// <summary>
    /// Implements business logic for chat operations.
    /// This layer is responsible for validation, mapping, and orchestration.
    /// It has NO knowledge of HTTP, SignalR, or SQL — pure application logic.
    /// </summary>
    public class ChatService : IChatService
    {
        private readonly IChatRepository _chatRepository;
        private readonly ILogger<ChatService> _logger;

        public ChatService(IChatRepository chatRepository, ILogger<ChatService> logger)
        {
            _chatRepository = chatRepository;
            _logger = logger;
        }

        public async Task<MessageDto> SendMessageAsync(SendMessageDto sendMessageDto)
        {
            // --- Validation ---
            if (string.IsNullOrWhiteSpace(sendMessageDto.SenderId))
                throw new ArgumentException("SenderId cannot be empty.", nameof(sendMessageDto.SenderId));

            if (string.IsNullOrWhiteSpace(sendMessageDto.ReceiverId))
                throw new ArgumentException("ReceiverId cannot be empty.", nameof(sendMessageDto.ReceiverId));

            if (string.IsNullOrWhiteSpace(sendMessageDto.MessageText))
                throw new ArgumentException("MessageText cannot be empty.", nameof(sendMessageDto.MessageText));

            if (sendMessageDto.MessageText.Length > 2000)
                throw new ArgumentException("MessageText exceeds maximum length of 2000 characters.");

            if (sendMessageDto.SenderId.Equals(sendMessageDto.ReceiverId, StringComparison.OrdinalIgnoreCase))
                throw new ArgumentException("Sender and Receiver cannot be the same user.");

            var message = new Message
            {
                SenderId    = sendMessageDto.SenderId.Trim(),
                ReceiverId  = sendMessageDto.ReceiverId.Trim(),
                MessageText = sendMessageDto.MessageText.Trim(),
                Timestamp   = DateTime.UtcNow
            };

            // --- Persist via Repository ---
            var newId = await _chatRepository.InsertMessageAsync(message);
            message.MessageId = newId;

            _logger.LogInformation(
                "Message sent: MessageId={MessageId}, From={SenderId}, To={ReceiverId}",
                message.MessageId, message.SenderId, message.ReceiverId);

            // --- Map Domain Entity → DTO and return ---
            return MapToDto(message);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<MessageDto>> GetChatHistoryAsync(string userA, string userB)
        {
            if (string.IsNullOrWhiteSpace(userA) || string.IsNullOrWhiteSpace(userB))
                throw new ArgumentException("Both user identifiers must be provided.");

            var messages = await _chatRepository.GetChatHistoryAsync(
                userA.Trim(), userB.Trim());

            return messages.Select(MapToDto);
        }

        /// <inheritdoc />
        public async Task CleanupOldMessagesAsync()
        {
            _logger.LogInformation("Running scheduled message cleanup (older than 12 hours)...");
            await _chatRepository.DeleteOldMessagesAsync(olderThanHours: 12);
            _logger.LogInformation("Message cleanup completed.");
        }

        // -----------------------------------------------------------------------
        // Private mapping helper — keeps mapping logic centralized
        // -----------------------------------------------------------------------
        private static MessageDto MapToDto(Message m) => new()
        {
            MessageId   = m.MessageId,
            SenderId    = m.SenderId,
            ReceiverId  = m.ReceiverId,
            MessageText = m.MessageText,
            Timestamp   = m.Timestamp
        };
    }
}
