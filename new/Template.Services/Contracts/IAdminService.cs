using System.Threading.Tasks;
using Template.Models.ServiceModels.Admin;
using Template.Models.ServiceModels.Admin.PermissionManagement;
using Template.Models.ServiceModels.Admin.UserManagement;

namespace Template.Services.Contracts
{
    public interface IAdminService
    {
        Task<GetUserManagementResponse> GetUserManagement();

        Task<DisableUserResponse> DisableUser(DisableUserRequest request);

        Task<EnableUserResponse> EnableUser(EnableUserRequest request);

        Task<UnlockUserResponse> UnlockUser(UnlockUserRequest request);

        Task<GetUserResponse> GetUser(GetUserRequest request);

        Task<UpdateUserResponse> UpdateUser(UpdateUserRequest request);

        Task<CreateUserResponse> CreateUser(CreateUserRequest request);


        Task<GetRoleManagementResponse> GetRoleManagement();

        Task<DisableRoleResponse> DisableRole(DisableRoleRequest request);

        Task<EnableRoleResponse> EnableRole(EnableRoleRequest request);

        Task<GetRoleResponse> GetRole(GetRoleRequest request);

        Task<UpdateRoleResponse> UpdateRole(UpdateRoleRequest request);

        Task<CreateRoleResponse> CreateRole(CreateRoleRequest request);


        Task<GetPermissionManagementResponse> GetPermissionManagement();

        Task<GetPermissionResponse> GetPermission(GetPermissionRequest request);

        Task<UpdatePermissionResponse> UpdatePermission(UpdatePermissionRequest request);

        Task<CreatePermissionResponse> CreatePermission(CreatePermissionRequest request);



        Task<GetConfigurationManagementResponse> GetConfigurationManagement();

        Task<GetConfigurationItemResponse> GetConfigurationItem(GetConfigurationItemRequest request);

        Task<UpdateConfigurationItemResponse> UpdateConfigurationItem(UpdateConfigurationItemRequest request);

        Task<CreateConfigurationItemResponse> CreateConfigurationItem(CreateConfigurationItemRequest request);


        Task<GetSessionsResponse> GetSessions(GetSessionsRequest request);

        Task<GetSessionResponse> GetSession(GetSessionRequest request);


        Task<GetSessionEventManagementResponse> GetSessionEventManagement();

        Task<GetSessionEventResponse> GetSessionEvent(GetSessionEventRequest request);

        Task<UpdateSessionEventResponse> UpdateSessionEvent(UpdateSessionEventRequest request);

        Task<CreateSessionEventResponse> CreateSessionEvent(CreateSessionEventRequest request);
    }
}
