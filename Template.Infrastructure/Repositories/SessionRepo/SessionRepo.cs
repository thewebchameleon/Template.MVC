using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Template.Infrastructure.Adapters;
using Template.Infrastructure.Configuration.Models;
using Template.Infrastructure.Repositories.SessionRepo.Contracts;
using Template.Infrastructure.Repositories.SessionRepo.Models;
using Template.Models.DomainModels;

namespace Template.Infrastructure.Repositories.SessionRepo
{
    public class SessionRepo : BaseSQLRepo, ISessionRepo
    {
        #region Instance Fields

        private IDbConnection _connection;
        private IDbTransaction _transaction;

        #endregion

        #region Constructor

        public SessionRepo(IDbConnection connection, IDbTransaction transaction, ConnectionStringSettings connectionStrings)
            : base(connectionStrings)
        {
            _connection = connection;
            _transaction = transaction;
        }

        #endregion

        #region Public Methods

        public async Task AddUserToSession(AddUserToSessionRequest request)
        {
            var sqlStoredProc = "sp_session_add_user_by_guid";

            var response = await DapperAdapter.GetFromStoredProcAsync<int>
                (
                    storedProcedureName: sqlStoredProc,
                    parameters: new
                    {
                        Guid = request.SessionGuid,
                        UserId = request.UserId,
                        UpdatedBy = request.UpdatedBy
                    },
                    dbconnectionString: DefaultConnectionString,
                    sqltimeout: DefaultTimeOut,
                    dbconnection: _connection,
                    dbtransaction: _transaction);

            if (response == null || response.FirstOrDefault() == 0)
            {
                throw new Exception("No items were created");
            }
        }

        public async Task CreateSession(CreateSessionRequest request)
        {
            var sqlStoredProc = "sp_session_create";

            var response = await DapperAdapter.GetFromStoredProcAsync<int>
                (
                    storedProcedureName: sqlStoredProc,
                    parameters: new
                    {
                        Guid = request.Guid,
                        CreatedBy = request.CreatedBy
                    },
                    dbconnectionString: DefaultConnectionString,
                    sqltimeout: DefaultTimeOut,
                    dbconnection: _connection,
                    dbtransaction: _transaction);

            if (response == null || response.FirstOrDefault() == 0)
            {
                throw new Exception("No items were created");
            }
        }

        public async Task DeleteSession(DeleteSessionRequest request)
        {
            var sqlStoredProc = "sp_session_delete";

            var response = await DapperAdapter.GetFromStoredProcAsync<int>
                (
                    storedProcedureName: sqlStoredProc,
                    parameters: new
                    {
                        Id = request.Id,
                        UpdatedBy = request.UpdatedBy
                    },
                    dbconnectionString: DefaultConnectionString,
                    sqltimeout: DefaultTimeOut,
                    dbconnection: _connection,
                    dbtransaction: _transaction);

            if (response == null || response.FirstOrDefault() == 0)
            {
                throw new Exception("No items were deleted");
            }
        }

        public async Task<Session> GetSessionByGuid(GetSessionByGuidRequest request)
        {
            var sqlStoredProc = "sp_session_get_by_guid";

            var response = await DapperAdapter.GetFromStoredProcAsync<Session>
                (
                    storedProcedureName: sqlStoredProc,
                    parameters: new
                    {
                        Guid = request.Guid
                    },
                    dbconnectionString: DefaultConnectionString,
                    sqltimeout: DefaultTimeOut,
                    dbconnection: _connection,
                    dbtransaction: _transaction);

            return response.FirstOrDefault();
        }

        public async Task<List<Session>> GetSessionsByUserId(GetSessionsByUserIdRequest request)
        {
            var sqlStoredProc = "sp_sessions_get_by_user_id";

            var response = await DapperAdapter.GetFromStoredProcAsync<Session>
                (
                    storedProcedureName: sqlStoredProc,
                    parameters: new
                    {
                        UserId = request.UserId
                    },
                    dbconnectionString: DefaultConnectionString,
                    sqltimeout: DefaultTimeOut,
                    dbconnection: _connection,
                    dbtransaction: _transaction);

            return response.ToList();
        }

        #endregion
    }
}
