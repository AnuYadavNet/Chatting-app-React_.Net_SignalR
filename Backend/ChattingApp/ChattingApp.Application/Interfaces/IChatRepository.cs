using ChattingApp.Domain.Entities;

namespace ChattingApp.Application.Interfaces
{
    /// <summary>
    /// Defines the data-access contract for chat messages.
    /// The Application layer depends on this abstraction — never on a concrete class.
    /// This is the Dependency Inversion principle in action.
    /// </summary>
    public interface IChatRepository
    {
        /// <summary>
        /// Persists a new message to the database via stored procedure.
        /// Returns the auto-generated MessageId assigned by SQL Server.
        /// </summary>
        Task<int> InsertMessageAsync(Message message);

        /// <summary>
        /// Retrieves the full conversation history between two users,
        /// ordered by Timestamp ascending (oldest first).
        /// </summary>
        Task<IEnumerable<Message>> GetChatHistoryAsync(string userA, string userB);

        /// <summary>
        /// Removes all messages older than the specified threshold (default 12 hours).
        /// Called by the background cleanup service.
        /// </summary>
        Task DeleteOldMessagesAsync(int olderThanHours = 12);
    }
}
