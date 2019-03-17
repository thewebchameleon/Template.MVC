using System.Threading.Tasks;
using Template.Models.ServiceModels.Session;

namespace Template.Services.Contracts
{
    public interface ISessionService
    {
        Task<GetSessionResponse> GetSession();

        Task<GetAuthenticatedSessionResponse> GetAuthenticatedSession();
    }
}
