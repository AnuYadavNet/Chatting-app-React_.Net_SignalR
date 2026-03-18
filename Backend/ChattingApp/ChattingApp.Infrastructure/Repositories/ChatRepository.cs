using ChattingApp.Application.Interfaces;
using ChattingApp.Domain.Entities;
using ChattingApp.Infrastructure.Data;
using Dapper;
using Microsoft.Extensions.Logging;
using System.Data;

namespace ChattingApp.Infrastructure.Repositories
{
    /// <summary>
    /// Concrete implementation of IChatRepository.
    /// Uses Dapper as a lightweight ORM to call SQL Server Stored Procedures.
    /// This class lives in Infrastructure — it knows about SQL but nothing about HTTP or SignalR.
    /// </summary>
    public class ChatRepository : IChatRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;
        private readonly ILogger<ChatRepository> _logger;

        public ChatRepository(IDbConnectionFactory connectionFactory, ILogger<ChatRepository> logger)
        {
            _connectionFactory = connectionFactory;
            _logger = logger;
        }

        /// <inheritdoc />
        public async Task<int> InsertMessageAsync(Message message)
        {
            try
            {
                using var connection = await _connectionFactory.CreateConnectionAsync();

                var parameters = new DynamicParameters();
                parameters.Add("@SenderId",    message.SenderId,    DbType.String);
                parameters.Add("@ReceiverId",  message.ReceiverId,  DbType.String);
                parameters.Add("@MessageText", message.MessageText, DbType.String);
                parameters.Add("@Timestamp",   message.Timestamp,   DbType.DateTime2);
                parameters.Add("@NewMessageId", dbType: DbType.Int32, direction: ParameterDirection.Output);

                await connection.ExecuteAsync(
                    "usp_InsertMessage",
                    parameters,
                    commandType: CommandType.StoredProcedure);

                return parameters.Get<int>("@NewMessageId");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to insert message. Sender={SenderId}", message.SenderId);
                throw;
            }
        }

        /// <inheritdoc />
        public async Task<IEnumerable<Message>> GetChatHistoryAsync(string userA, string userB)
        {
            try
            {
                using var connection = await _connectionFactory.CreateConnectionAsync();

                var parameters = new DynamicParameters();
                parameters.Add("@UserA", userA, DbType.String);
                parameters.Add("@UserB", userB, DbType.String);

                var results = await connection.QueryAsync<Message>(
                    "usp_GetChatHistory",
                    parameters,
                    commandType: CommandType.StoredProcedure);

                return results;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch chat history between {UserA} and {UserB}", userA, userB);
                throw;
            }
        }

        /// <inheritdoc />
        public async Task DeleteOldMessagesAsync(int olderThanHours = 12)
        {
            try
            {
                using var connection = await _connectionFactory.CreateConnectionAsync();

                var parameters = new DynamicParameters();
                parameters.Add("@OlderThanHours", olderThanHours, DbType.Int32);

                await connection.ExecuteAsync(
                    "usp_DeleteOldMessages",
                    parameters,
                    commandType: CommandType.StoredProcedure,
                    commandTimeout: 120); // allow up to 2 min for large deletes

                _logger.LogInformation("Old messages deleted (threshold: {Hours}h)", olderThanHours);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete old messages.");
                throw;
            }
        }
    }
}
