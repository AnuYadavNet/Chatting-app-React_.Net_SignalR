using ChattingApp.Application.Interfaces;
using ChattingApp.Domain.Entities;
using ChattingApp.Infrastructure.Data;
using Dapper;
using Microsoft.Extensions.Logging;
using System.Data;

namespace ChattingApp.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;
        private readonly ILogger<UserRepository> _logger;

        public UserRepository(IDbConnectionFactory connectionFactory, ILogger<UserRepository> logger)
        {
            _connectionFactory = connectionFactory;
            _logger = logger;
        }

        public async Task<int> CreateUserAsync(User user)
        {
            try
            {
                using var connection = await _connectionFactory.CreateConnectionAsync();

                var parameters = new DynamicParameters();
                parameters.Add("@Username", user.Username, DbType.String);
                parameters.Add("@PasswordHash", user.PasswordHash, DbType.String);
                parameters.Add("@Role", user.Role, DbType.String);
                parameters.Add("@NewUserId", dbType: DbType.Int32, direction: ParameterDirection.Output);

                await connection.ExecuteAsync(
                    "usp_CreateUser",
                    parameters,
                    commandType: CommandType.StoredProcedure);

                return parameters.Get<int>("@NewUserId");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create user. Username={Username}", user.Username);
                throw;
            }
        }

        public async Task<User?> GetUserByUsernameAsync(string username)
        {
            try
            {
                using var connection = await _connectionFactory.CreateConnectionAsync();

                var parameters = new DynamicParameters();
                parameters.Add("@Username", username, DbType.String);

                var user = await connection.QuerySingleOrDefaultAsync<User>(
                    "usp_GetUserByUsername",
                    parameters,
                    commandType: CommandType.StoredProcedure);

                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch user {Username}", username);
                throw;
            }
        }
    }
}
