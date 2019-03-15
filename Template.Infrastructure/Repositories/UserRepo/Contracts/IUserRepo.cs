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

        Task DeleteRole(DeleteRoleRequest request);

        Task<int> CreateUser(CreateUserRequest request);

        Task DeleteUser(DeleteUserRequest request);

        Task UpdateUser(UpdateUserRequest request);

        Task<int> CreateUserRole(CreateUserRoleRequest request);

        Task DeleteUserRole(DeleteUserRoleRequest request);

        Task ActivateAccount(ActivateAccountRequest request);

        Task CreateToken(CreateTokenRequest request);

        Task<FetchDuplicateUserResponse> FetchDuplicateUser(FetchDuplicateUserRequest request);

        Task<List<User>> GetUsers();

        Task<List<Role>> GetRoles();

        Task<List<RoleClaim>> GetRoleClaims();

        Task<List<Token>> GetTokens();

        Task<List<UserRole>> GetUserRoles();

        Task<List<Claim>> GetClaims();

        Task<List<UserClaim>> GetUserClaims();

        Task<int> CreateRoleClaim(CreateRoleClaimRequest request);

        Task<int> CreateUserClaim(CreateUserClaimRequest request);

        Task DeleteRoleClaim(DeleteRoleClaimRequest request);

        Task DeleteUserClaim(DeleteUserClaimRequest request);
    }
}
