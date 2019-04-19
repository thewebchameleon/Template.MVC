using System.Threading.Tasks;
using Template.Models.ServiceModels;
using Template.Models.ServiceModels.Account;

namespace Template.Services.Contracts
{
    public interface IAccountService
    {
        Task<LoginResponse> Login(LoginRequest request);

        Task<RegisterResponse> Register(RegisterRequest request);

        Task<ActivateAccountResponse> ActivateAccount(ActivateAccountRequest request);

        Task<ValidateResetPasswordTokenResponse> ValidateResetPasswordToken(ValidateResetPasswordTokenRequest request);

        Task<ResetPasswordResponse> ResetPassword(ResetPasswordRequest request);

        Task<ForgotPasswordResponse> ForgotPassword(ForgotPasswordRequest request);

        void Logout();

        Task<GetProfileResponse> GetProfile();

        Task<UpdateProfileResponse> UpdateProfile(UpdateProfileRequest request);

        Task<DuplicateUserCheckResponse> DuplicateUserCheck(DuplicateUserCheckRequest request);

        Task<DuplicateRoleCheckResponse> DuplicateRoleCheck(DuplicateRoleCheckRequest request);
    }
}
