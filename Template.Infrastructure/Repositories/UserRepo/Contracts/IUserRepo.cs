using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Template.Infrastructure.Repositories.UserRepo.Models;
using Template.Models.DomainModels;

namespace Template.Infrastructure.Repositories.UserRepo.Contracts
{
    public interface IUserRepo
    {
        Task<int> CreateRole(CreateRoleRequest request);

        Task UpdateRole(UpdateRoleRequest request);

        Task DisableRole(DisableRoleRequest request);

        Task EnableRole(EnableRoleRequest request);

        Task<int> CreateUser(CreateUserRequest request);

        Task DisableUser(DisableUserRequest request);

        Task EnableUser(EnableUserRequest request);

        Task UpdateUser(UpdateUserRequest request);

        Task<int> CreateUserRole(CreateUserRoleRequest request);

        Task DeleteUserRole(DeleteUserRoleRequest request);

        Task ActivateAccount(ActivateAccountRequest request);

        Task CreateToken(CreateTokenRequest request);

        Task<UserEntity> FetchDuplicateUser(FetchDuplicateUserRequest request);

        Task<List<UserEntity>> GetUsers();

        Task<List<RoleEntity>> GetRoles();

        Task<List<RoleClaim>> GetRoleClaims();

        Task<List<TokenEntity>> GetTokens();

        Task<List<UserRoleEntity>> GetUserRoles();

        Task<List<ClaimEntity>> GetClaims();

        Task<int> CreateRoleClaim(CreateRoleClaimRequest request);

        Task DeleteRoleClaim(DeleteRoleClaimRequest request);

        Task<UserEntity> GetUserById(GetUserByIdRequest request);

        Task<UserEntity> GetUserByUsername(GetUserByUsernameRequest request);

        Task<UserEntity> GetUserByEmail(GetUserByEmailRequest request);

        Task<UserEntity> GetUserByMobileNumber(GetUserByMobileNumberRequest request);
    }
}
