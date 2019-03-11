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

        Task<ForgotPasswordResponse> ForgotPassword(ForgotPasswordRequest request);

        Task<GetProfileResponse> GetProfile(GetProfileRequest request);

        Task<UpdateProfileResponse> UpdateProfile(UpdateProfileRequest request);
    }
}
