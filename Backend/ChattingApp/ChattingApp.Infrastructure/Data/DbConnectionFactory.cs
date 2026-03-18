using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace ChattingApp.Infrastructure.Data
{
    /// <summary>
    /// Provides a factory for creating SqlConnection instances.
    /// Centralises connection string retrieval so it never leaks into repositories.
    /// Registered as Scoped — one factory per request/scope.
    /// </summary>
    public interface IDbConnectionFactory
    {
        /// <summary>Creates and returns an open IDbConnection.</summary>
        Task<IDbConnection> CreateConnectionAsync();
    }

    public class SqlConnectionFactory : IDbConnectionFactory
    {
        private readonly string _connectionString;

        public SqlConnectionFactory(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("ChattingAppDb")
                ?? throw new InvalidOperationException(
                    "Connection string 'ChattingAppDb' is not configured in appsettings.");
        }

        public async Task<IDbConnection> CreateConnectionAsync()
        {
            var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            return connection;
        }
    }
}
