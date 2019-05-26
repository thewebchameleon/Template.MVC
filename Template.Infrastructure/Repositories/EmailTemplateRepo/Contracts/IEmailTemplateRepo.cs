using System.Collections.Generic;
using System.Threading.Tasks;
using Template.Models.DomainModels;

namespace Template.Infrastructure.Repositories.Contracts
{
    public interface IEmailTemplateRepo
    {
        Task<List<EmailTemplateEntity>> GetEmailTemplates();
    }
}
