using MySql.Data.MySqlClient;
using System.Data.SqlClient;
using Template.Infrastructure.Configuration.Models;
using Template.Infrastructure.UnitOfWork.Contracts;

namespace Template.Infrastructure.UnitOfWork
{
    public class UnitOfWorkFactory : IUnitOfWorkFactory
    {
        private readonly ConnectionStringSettings _connectionSettings;

        public UnitOfWorkFactory(ConnectionStringSettings connectionSettings) : base()
        {
            _connectionSettings = connectionSettings;
        }

        public IUnitOfWork GetUnitOfWork(bool beginTransaction = true)
        {
            var connectionString = _connectionSettings.DefaultConnection;
            return new UnitOfWork(new SqlConnection(connectionString), _connectionSettings, beginTransaction);
        }

        public IUnitOfWork GetMySQLUnitOfWork(bool beginTransaction = true)
        {
            var connectionString = _connectionSettings.DefaultConnection;
            return new UnitOfWork(new MySqlConnection(connectionString), _connectionSettings, beginTransaction);
        }
    }
}
