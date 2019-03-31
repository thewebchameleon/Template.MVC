using System.Threading.Tasks;
using Template.Models.ServiceModels;
using Template.Models.ServiceModels.Account;

namespace Template.Services.Contracts
{
    public interface IAccountService
    {
        Task<RegisterResponse> Register(RegisterRequest request);

        Task<LoginResponse> Login(LoginRequest request);

        void Logout();

        Task<GetProfileResponse> GetProfile();

        Task<UpdateProfileResponse> UpdateProfile(UpdateProfileRequest request);

        Task<DuplicateUserCheckResponse> DuplicateUserCheck(DuplicateUserCheckRequest request);

        Task<DuplicateRoleCheckResponse> DuplicateRoleCheck(DuplicateRoleCheckRequest request);
    }
}
