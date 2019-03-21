using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Template.Infrastructure.Adapters;
using Template.Infrastructure.Configuration.Models;
using Template.Infrastructure.Repositories.UserRepo.Contracts;
using Template.Infrastructure.Repositories.UserRepo.Models;
using Template.Models.DomainModels;

namespace Template.Infrastructure.Repositories.UserRepo
{
    public class UserRepo : BaseSQLRepo, IUserRepo
    {
        #region Instance Fields

        private IDbConnection _connection;
        private IDbTransaction _transaction;

        #endregion

        #region Constructor

        public UserRepo(IDbConnection connection, IDbTransaction transaction, ConnectionStringSettings connectionStrings)
            : base(connectionStrings)
        {
            _connection = connection;
            _transaction = transaction;
        }

        #endregion

        #region Public Methods

        public async Task ActivateAccount(ActivateAccountRequest request)
        {
            var sqlStoredProc = "sp_user_activate_account";

            var response = await DapperAdapter.GetFromStoredProcAsync<int>
                (
                    storedProcedureName: sqlStoredProc,
                    parameters: request,
                    dbconnectionString: DefaultConnectionString,
                    sqltimeout: DefaultTimeOut,
                    dbconnection: _connection,
                    dbtransaction: _transaction);

            if (response == null || response.First() == 0)
            {
                throw new Exception("No items have been updated");
            }
        }

        public async Task CreateToken(CreateTokenRequest request)
        {
            var sqlStoredProc = "sp_user_token_create";

            var response = await DapperAdapter.GetFromStoredProcAsync<int>
                (
                    storedProcedureName: sqlStoredProc,
                    parameters: request,
                    dbconnectionString: DefaultConnectionString,
                    sqltimeout: DefaultTimeOut,
                    dbconnection: _connection,
                    dbtransaction: _transaction);

            if (response == null || response.First() == 0)
            {
                throw new System.Exception("No items have been created");
            }
        }

        public async Task<UserEntity> FetchDuplicateUser(FetchDuplicateUserRequest request)
        {
            var sqlStoredProc = "sp_user_duplicate_check";

            var response = await DapperAdapter.GetFromStoredProcAsync<UserEntity>
                (
                    storedProcedureName: sqlStoredProc,
                    parameters: request,
                    dbconnectionString: DefaultConnectionString,
                    sqltimeout: DefaultTimeOut,
                    dbconnection: _connection,
                    dbtransaction: _transaction);

            return response.FirstOrDefault();
        }

        public async Task<List<UserEntity>> GetUsers()
        {
            var sqlStoredProc = "sp_users_get";

            var response = await DapperAdapter.GetFromStoredProcAsync<UserEntity>
                (
                    storedProcedureName: sqlStoredProc,
                    parameters: new { },
                    dbconnectionString: DefaultConnectionString,
                    sqltimeout: DefaultTimeOut,
                    dbconnection: _connection,
                    dbtransaction: _transaction);

            return response.ToList();
        }

        public async Task<List<RoleEntity>> GetRoles()
        {
            var sqlStoredProc = "sp_roles_get";

            var response = await DapperAdapter.GetFromStoredProcAsync<RoleEntity>
                (
                    storedProcedureName: sqlStoredProc,
                    parameters: new { },
                    dbconnectionString: DefaultConnectionString,
                    sqltimeout: DefaultTimeOut,
                    dbconnection: _connection,
                    dbtransaction: _transaction);

            return response.ToList();
        }

        public async Task<List<RoleClaim>> GetRoleClaims()
        {
            var sqlStoredProc = "sp_role_claims_get";

            var response = await DapperAdapter.GetFromStoredProcAsync<RoleClaim>
                (
                    storedProcedureName: sqlStoredProc,
                    parameters: new { },
                    dbconnectionString: DefaultConnectionString,
                    sqltimeout: DefaultTimeOut,
                    dbconnection: _connection,
                    dbtransaction: _transaction);

            return response.ToList();
        }

        public async Task<List<TokenEntity>> GetTokens()
        {
            var sqlStoredProc = "sp_user_tokens_get";

            var response = await DapperAdapter.GetFromStoredProcAsync<TokenEntity>
                (
                    storedProcedureName: sqlStoredProc,
                    parameters: new { },
                    dbconnectionString: DefaultConnectionString,
                    sqltimeout: DefaultTimeOut,
                    dbconnection: _connection,
                    dbtransaction: _transaction);

            return response.ToList();
        }

        public async Task<List<UserRoleEntity>> GetUserRoles()
        {
            var sqlStoredProc = "sp_user_roles_get";

            var response = await DapperAdapter.GetFromStoredProcAsync<UserRoleEntity>
                (
                    storedProcedureName: sqlStoredProc,
                    parameters: new { },
                    dbconnectionString: DefaultConnectionString,
                    sqltimeout: DefaultTimeOut,
                    dbconnection: _connection,
                    dbtransaction: _transaction);

            return response.ToList();
        }

        public async Task<int> CreateRole(CreateRoleRequest request)
        {
            var sqlStoredProc = "sp_role_create";

            var response = await DapperAdapter.GetFromStoredProcAsync<int>
                (
                    storedProcedureName: sqlStoredProc,
                    parameters: request,
                    dbconnectionString: DefaultConnectionString,
                    sqltimeout: DefaultTimeOut,
                    dbconnection: _connection,
                    dbtransaction: _transaction);

            if (response == null || response.First() == 0)
            {
                throw new Exception("No items were created");
            }
            return response.FirstOrDefault();
        }

        public async Task UpdateRole(UpdateRoleRequest request)
        {
            var sqlStoredProc = "sp_role_update";

            var response = await DapperAdapter.GetFromStoredProcAsync<int>
                (
                    storedProcedureName: sqlStoredProc,
                    parameters: request,
                    dbconnectionString: DefaultConnectionString,
                    sqltimeout: DefaultTimeOut,
                    dbconnection: _connection,
                    dbtransaction: _transaction);

            if (response == null || response.First() == 0)
            {
                throw new Exception("No items have been updated");
            }
        }

        public async Task DisableRole(DisableRoleRequest request)
        {
            var sqlStoredProc = "sp_role_disable";

            var response = await DapperAdapter.GetFromStoredProcAsync<int>
                (
                    storedProcedureName: sqlStoredProc,
                    parameters: request,
                    dbconnectionString: DefaultConnectionString,
                    sqltimeout: DefaultTimeOut,
                    dbconnection: _connection,
                    dbtransaction: _transaction);

            if (response == null || response.First() == 0)
            {
                throw new Exception("No items have been disabled");
            }
        }

        public async Task<int> CreateUser(CreateUserRequest request)
        {
            var sqlStoredProc = "sp_user_create";

            var response = await DapperAdapter.GetFromStoredProcAsync<int>
                (
                    storedProcedureName: sqlStoredProc,
                    parameters: request,
                    dbconnectionString: DefaultConnectionString,
                    sqltimeout: DefaultTimeOut,
                    dbconnection: _connection,
                    dbtransaction: _transaction);

            if (response == null || response.First() == 0)
            {
                throw new Exception("No items have been created");
            }
            return response.FirstOrDefault();
        }

        public async Task DisableUser(DisableUserRequest request)
        {
            var sqlStoredProc = "sp_user_disable";

            var response = await DapperAdapter.GetFromStoredProcAsync<int>
                (
                    storedProcedureName: sqlStoredProc,
                    parameters: request,
                    dbconnectionString: DefaultConnectionString,
                    sqltimeout: DefaultTimeOut,
                    dbconnection: _connection,
                    dbtransaction: _transaction);

            if (response == null || response.First() == 0)
            {
                throw new Exception("No items have been disabled");
            }
        }

        public async Task UpdateUser(UpdateUserRequest request)
        {
            var sqlStoredProc = "sp_user_update";

            var response = await DapperAdapter.GetFromStoredProcAsync<int>
                (
                    storedProcedureName: sqlStoredProc,
                    parameters: request,
                    dbconnectionString: DefaultConnectionString,
                    sqltimeout: DefaultTimeOut,
                    dbconnection: _connection,
                    dbtransaction: _transaction);

            if (response == null || response.First() == 0)
            {
                throw new Exception("No items have been updated");
            }
        }

        public async Task<int> CreateUserRole(CreateUserRoleRequest request)
        {
            var sqlStoredProc = "sp_user_role_create";

            var response = await DapperAdapter.GetFromStoredProcAsync<int>
                (
                    storedProcedureName: sqlStoredProc,
                    parameters: request,
                    dbconnectionString: DefaultConnectionString,
                    sqltimeout: DefaultTimeOut,
                    dbconnection: _connection,
                    dbtransaction: _transaction);

            if (response == null || response.FirstOrDefault() == 0)
            {
                throw new Exception("No items were created");
            }
            return response.First();
        }

        public async Task DeleteUserRole(DeleteUserRoleRequest request)
        {
            var sqlStoredProc = "sp_user_role_delete";

            var response = await DapperAdapter.GetFromStoredProcAsync<int>
                (
                    storedProcedureName: sqlStoredProc,
                    parameters: request,
                    dbconnectionString: DefaultConnectionString,
                    sqltimeout: DefaultTimeOut,
                    dbconnection: _connection,
                    dbtransaction: _transaction);

            if (response == null || response.FirstOrDefault() == 0)
            {
                throw new Exception("No items were deleted");
            }
        }

        public async Task<int> CreateRoleClaim(CreateRoleClaimRequest request)
        {
            var sqlStoredProc = "sp_role_claim_create";

            var response = await DapperAdapter.GetFromStoredProcAsync<int>
                (
                    storedProcedureName: sqlStoredProc,
                    parameters: request,
                    dbconnectionString: DefaultConnectionString,
                    sqltimeout: DefaultTimeOut,
                    dbconnection: _connection,
                    dbtransaction: _transaction);

            if (response == null || response.First() == 0)
            {
                throw new Exception("No items have been created");
            }
            return response.First();
        }

        public async Task DeleteRoleClaim(DeleteRoleClaimRequest request)
        {
            var sqlStoredProc = "sp_role_claim_delete";

            var response = await DapperAdapter.GetFromStoredProcAsync<int>
                (
                    storedProcedureName: sqlStoredProc,
                    parameters: request,
                    dbconnectionString: DefaultConnectionString,
                    sqltimeout: DefaultTimeOut,
                    dbconnection: _connection,
                    dbtransaction: _transaction);

            if (response == null || response.First() == 0)
            {
                throw new Exception("No items have been deleted");
            }
        }

        public async Task<List<ClaimEntity>> GetClaims()
        {
            var sqlStoredProc = "sp_claims_get";

            var response = await DapperAdapter.GetFromStoredProcAsync<ClaimEntity>
                (
                    storedProcedureName: sqlStoredProc,
                    parameters: new { },
                    dbconnectionString: DefaultConnectionString,
                    sqltimeout: DefaultTimeOut,
                    dbconnection: _connection,
                    dbtransaction: _transaction);

            return response.ToList();
        }

        public async Task<UserEntity> GetUserById(GetUserByIdRequest request)
        {
            var sqlStoredProc = "sp_user_get_by_id";

            var response = await DapperAdapter.GetFromStoredProcAsync<UserEntity>
                (
                    storedProcedureName: sqlStoredProc,
                    parameters: request,
                    dbconnectionString: DefaultConnectionString,
                    sqltimeout: DefaultTimeOut,
                    dbconnection: _connection,
                    dbtransaction: _transaction);

            return response.FirstOrDefault();
        }

        public async Task<UserEntity> GetUserByUsername(GetUserByUsernameRequest request)
        {
            var sqlStoredProc = "sp_user_get_by_username";

            var response = await DapperAdapter.GetFromStoredProcAsync<UserEntity>
                (
                    storedProcedureName: sqlStoredProc,
                    parameters: request,
                    dbconnectionString: DefaultConnectionString,
                    sqltimeout: DefaultTimeOut,
                    dbconnection: _connection,
                    dbtransaction: _transaction);

            return response.FirstOrDefault();
        }

        public async Task<UserEntity> GetUserByEmail(GetUserByEmailRequest request)
        {
            var sqlStoredProc = "sp_user_get_by_email";

            var response = await DapperAdapter.GetFromStoredProcAsync<UserEntity>
                (
                    storedProcedureName: sqlStoredProc,
                    parameters: request,
                    dbconnectionString: DefaultConnectionString,
                    sqltimeout: DefaultTimeOut,
                    dbconnection: _connection,
                    dbtransaction: _transaction);

            return response.FirstOrDefault();
        }

        public async Task<UserEntity> GetUserByMobileNumber(GetUserByMobileNumberRequest request)
        {
            var sqlStoredProc = "sp_user_get_by_mobile_number";

            var response = await DapperAdapter.GetFromStoredProcAsync<UserEntity>
                (
                    storedProcedureName: sqlStoredProc,
                    parameters: request,
                    dbconnectionString: DefaultConnectionString,
                    sqltimeout: DefaultTimeOut,
                    dbconnection: _connection,
                    dbtransaction: _transaction);

            return response.FirstOrDefault();
        }

        public async Task EnableRole(EnableRoleRequest request)
        {
            var sqlStoredProc = "sp_role_enable";

            var response = await DapperAdapter.GetFromStoredProcAsync<int>
                (
                    storedProcedureName: sqlStoredProc,
                    parameters: request,
                    dbconnectionString: DefaultConnectionString,
                    sqltimeout: DefaultTimeOut,
                    dbconnection: _connection,
                    dbtransaction: _transaction);

            if (response == null || response.First() == 0)
            {
                throw new Exception("No items have been enabled");
            }
        }

        public async Task EnableUser(EnableUserRequest request)
        {
            var sqlStoredProc = "sp_user_enable";

            var response = await DapperAdapter.GetFromStoredProcAsync<int>
                (
                    storedProcedureName: sqlStoredProc,
                    parameters: request,
                    dbconnectionString: DefaultConnectionString,
                    sqltimeout: DefaultTimeOut,
                    dbconnection: _connection,
                    dbtransaction: _transaction);

            if (response == null || response.First() == 0)
            {
                throw new Exception("No items have been enabled");
            }
        }

        public async Task UpdateClaim(UpdateClaimRequest request)
        {
            var sqlStoredProc = "sp_claim_update";

            var response = await DapperAdapter.GetFromStoredProcAsync<int>
                (
                    storedProcedureName: sqlStoredProc,
                    parameters: request,
                    dbconnectionString: DefaultConnectionString,
                    sqltimeout: DefaultTimeOut,
                    dbconnection: _connection,
                    dbtransaction: _transaction);

            if (response == null || response.FirstOrDefault() == 0)
            {
                throw new Exception("No items have been updated");
            }
        }

        public async Task<int> CreateClaim(CreateClaimRequest request)
        {
            var sqlStoredProc = "sp_claim_create";

            var response = await DapperAdapter.GetFromStoredProcAsync<int>
                (
                    storedProcedureName: sqlStoredProc,
                    parameters: request,
                    dbconnectionString: DefaultConnectionString,
                    sqltimeout: DefaultTimeOut,
                    dbconnection: _connection,
                    dbtransaction: _transaction);

            if (response == null || response.FirstOrDefault() == 0)
            {
                throw new Exception("No items have been created");
            }
            return response.FirstOrDefault();
        }

        #endregion
    }
}
