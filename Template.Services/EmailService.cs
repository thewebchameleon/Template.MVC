using System.Threading.Tasks;
using Template.Models.ServiceModels;
using Template.Services.Contracts;

namespace Template.Services
{
    public class EmailService : IEmailService
    {
        public async Task SendAccountActivationEmail(SendAccountActivationEmailRequest request)
        {
            //throw new NotImplementedException();
        }
    }
}
