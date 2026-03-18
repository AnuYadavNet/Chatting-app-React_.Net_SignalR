namespace ChattingApp.Domain.Entities
{
    /// <summary>
    /// Represents a chat message in the domain layer.
    /// This is the core entity — no dependencies on external libraries.
    /// </summary>
    public class Message
    {
        public int MessageId { get; set; }

        /// <summary>The user who sent the message.</summary>
        public string SenderId { get; set; } = string.Empty;

        /// <summary>The user who should receive the message.</summary>
        public string ReceiverId { get; set; } = string.Empty;

        /// <summary>The actual text content of the message.</summary>
        public string MessageText { get; set; } = string.Empty;

        /// <summary>UTC timestamp when the message was created.</summary>
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// Determines if this message has expired (older than 12 hours).
        /// Used for soft-check logic if needed outside the scheduled job.
        /// </summary>
        public bool IsExpired => DateTime.UtcNow - Timestamp > TimeSpan.FromHours(12);
    }
}
