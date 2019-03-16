using System.Threading.Tasks;
using Template.Models.ServiceModels.Admin;

namespace Template.Services.Contracts
{
    public interface IAdminService
    {
        Task<GetUserManagementResponse> GetUserManagement(GetUserManagementRequest request);

        Task<DisableUserResponse> DisableUser(DisableUserRequest request, int userId);

        Task<EnableUserResponse> EnableUser(EnableUserRequest request, int userId);

        Task<GetUserResponse> GetUser(GetUserRequest request);

        Task<UpdateUserResponse> UpdateUser(UpdateUserRequest request, int userId);

        Task<CreateUserResponse> CreateUser(CreateUserRequest request, int userId);


        Task<GetRoleManagementResponse> GetRoleManagement(GetRoleManagementRequest request);

        Task<DisableRoleResponse> DisableRole(DisableRoleRequest request, int userId);

        Task<EnableRoleResponse> EnableRole(EnableRoleRequest request, int userId);

        Task<GetRoleResponse> GetRole(GetRoleRequest request);

        Task<UpdateRoleResponse> UpdateRole(UpdateRoleRequest request, int userId);

        Task<CreateRoleResponse> CreateRole(CreateRoleRequest request, int userId);
    }
}
