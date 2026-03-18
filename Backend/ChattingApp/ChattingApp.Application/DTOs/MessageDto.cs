namespace ChattingApp.Application.DTOs
{
    /// <summary>
    /// Data Transfer Object used to carry message data
    /// between SignalR Hub / Controller and the Service layer.
    /// Keeps the API contract decoupled from the domain entity.
    /// </summary>
    public class MessageDto
    {
        public int MessageId { get; set; }
        public string SenderId { get; set; } = string.Empty;
        public string ReceiverId { get; set; } = string.Empty;
        public string MessageText { get; set; } = string.Empty;

        /// <summary>Stored and returned as UTC; formatted on the frontend.</summary>
        public DateTime Timestamp { get; set; }
    }

    /// <summary>
    /// DTO used when a client sends a new message through SignalR.
    /// Contains only the fields the client needs to supply.
    /// </summary>
    public class SendMessageDto
    {
        public string SenderId { get; set; } = string.Empty;
        public string ReceiverId { get; set; } = string.Empty;
        public string MessageText { get; set; } = string.Empty;
    }
}
