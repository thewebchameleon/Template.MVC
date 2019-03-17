using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Template.Infrastructure.Adapters;
using Template.Infrastructure.Configuration.Models;
using Template.Infrastructure.Repositories.SessionRepo.Contracts;
using Template.Infrastructure.Repositories.SessionRepo.Models;

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

        public async Task<Template.Models.DomainModels.Session> AddUserToSession(AddUserToSessionRequest request)
        {
            var sqlStoredProc = "sp_session_add_user_id";

            var response = await DapperAdapter.GetFromStoredProcAsync<Template.Models.DomainModels.Session>
                (
                    storedProcedureName: sqlStoredProc,
                    parameters: request,
                    dbconnectionString: DefaultConnectionString,
                    sqltimeout: DefaultTimeOut,
                    dbconnection: _connection,
                    dbtransaction: _transaction);

            return response.FirstOrDefault();
        }

        public async Task<Template.Models.DomainModels.Session> CreateSession(CreateSessionRequest request)
        {
            var sqlStoredProc = "sp_session_create";

            var response = await DapperAdapter.GetFromStoredProcAsync<Template.Models.DomainModels.Session>
                (
                    storedProcedureName: sqlStoredProc,
                    parameters: request,
                    dbconnectionString: DefaultConnectionString,
                    sqltimeout: DefaultTimeOut,
                    dbconnection: _connection,
                    dbtransaction: _transaction);

            return response.FirstOrDefault();
        }

        public async Task<List<Template.Models.DomainModels.Session>> GetSessionsByStartDate(GetSessionsByStartDateRequest request)
        {
            var sqlStoredProc = "sp_sessions_get_by_start_date";

            var response = await DapperAdapter.GetFromStoredProcAsync<Template.Models.DomainModels.Session>
                (
                    storedProcedureName: sqlStoredProc,
                    parameters: request,
                    dbconnectionString: DefaultConnectionString,
                    sqltimeout: DefaultTimeOut,
                    dbconnection: _connection,
                    dbtransaction: _transaction);

            return response.ToList();
        }

        public async Task<List<Template.Models.DomainModels.Session>> GetSessionsByUserId(GetSessionsByUserIdRequest request)
        {
            var sqlStoredProc = "sp_sessions_get_by_user_id";

            var response = await DapperAdapter.GetFromStoredProcAsync<Template.Models.DomainModels.Session>
                (
                    storedProcedureName: sqlStoredProc,
                    parameters: request,
                    dbconnectionString: DefaultConnectionString,
                    sqltimeout: DefaultTimeOut,
                    dbconnection: _connection,
                    dbtransaction: _transaction);

            return response.ToList();
        }

        #endregion
    }
}
