using System.Threading.Tasks;
using Template.Models.ServiceModels.Admin;

namespace Template.Services.Contracts
{
    public interface IAdminService
    {
        Task<GetUserManagementResponse> GetUserManagement();

        Task<DisableUserResponse> DisableUser(DisableUserRequest request);

        Task<EnableUserResponse> EnableUser(EnableUserRequest request);

        Task<GetUserResponse> GetUser(GetUserRequest request);

        Task<UpdateUserResponse> UpdateUser(UpdateUserRequest request);

        Task<CreateUserResponse> CreateUser(CreateUserRequest request);


        Task<GetRoleManagementResponse> GetRoleManagement();

        Task<DisableRoleResponse> DisableRole(DisableRoleRequest request);

        Task<EnableRoleResponse> EnableRole(EnableRoleRequest request);

        Task<GetRoleResponse> GetRole(GetRoleRequest request);

        Task<UpdateRoleResponse> UpdateRole(UpdateRoleRequest request);

        Task<CreateRoleResponse> CreateRole(CreateRoleRequest request);


        Task<GetConfigurationManagementResponse> GetConfigurationManagement();

        Task<GetConfigurationItemResponse> GetConfigurationItem(GetConfigurationItemRequest request);

        Task<UpdateConfigurationItemResponse> UpdateConfigurationItem(UpdateConfigurationItemRequest request);

        Task<CreateConfigurationItemResponse> CreateConfigurationItem(CreateConfigurationItemRequest request);


        Task<GetSessionsResponse> GetSessions(GetSessionsRequest request);

        Task<GetSessionResponse> GetSession(GetSessionRequest request);
    }
}
