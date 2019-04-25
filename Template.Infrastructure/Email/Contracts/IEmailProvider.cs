using System.Threading.Tasks;
using Template.Infrastructure.Email.Models;

namespace Template.Infrastructure.Email.Contracts
{
    public interface IEmailProvider
    {
        Task Send(SendRequest request);
    }
}
