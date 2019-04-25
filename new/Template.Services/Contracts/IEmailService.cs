using System.Threading.Tasks;
using Template.Models.ServiceModels.Email;

namespace Template.Services.Contracts
{
    public interface IEmailService
    {
        Task SendAccountActivation(SendAccountActivationRequest request);

        Task SendResetPassword(SendResetPasswordRequest request);
    }
}
