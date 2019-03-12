using System.Threading.Tasks;
using Template.Models.ServiceModels.Admin;

namespace Template.Services.Contracts
{
    public interface IAdminService
    {
        Task<GetUserManagementResponse> GetUserManagement(GetUserManagementRequest request);

        Task<CreateOrUpdateUserResponse> CreateOrUpdateUser(CreateOrUpdateUserRequest request);

        Task<DeactivateUserResponse> DeactivateUser(DeactivateUserRequest request);

        Task<ActivateUserResponse> ActivateUser(ActivateUserRequest request);

        Task<GetUserResponse> GetUser(GetUserRequest request);

        Task<UpdateUserResponse> UpdateUser(UpdateUserRequest request);

        Task<CreateUserResponse> CreateUser(CreateUserRequest request);
    }
}
