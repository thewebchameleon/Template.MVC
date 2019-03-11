using System.Threading.Tasks;
using Template.Models.ServiceModels;

namespace Template.Services.Contracts
{
    public interface IAccountService
    {
        Task<RegisterResponse> Register(RegisterRequest request);

        Task<LoginResponse> Login(LoginRequest request);

        void Logout();

        Task<ForgotPasswordResponse> ForgotPassword(ForgotPasswordRequest request);
    }
}
