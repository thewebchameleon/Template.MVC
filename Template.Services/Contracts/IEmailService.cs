using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Template.Models.ServiceModels;

namespace Template.Services.Contracts
{
    public interface IEmailService
    {
        Task SendAccountActivationEmail(SendAccountActivationEmailRequest request);
    }
}
