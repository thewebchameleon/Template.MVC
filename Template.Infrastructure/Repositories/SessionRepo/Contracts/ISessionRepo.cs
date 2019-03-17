using System.Collections.Generic;
using System.Threading.Tasks;
using Template.Infrastructure.Repositories.SessionRepo.Models;

namespace Template.Infrastructure.Repositories.SessionRepo.Contracts
{
    public interface ISessionRepo
    {
        Task<Template.Models.DomainModels.Session> AddUserToSession(AddUserToSessionRequest request);

        Task<Template.Models.DomainModels.Session> CreateSession(CreateSessionRequest request);

        Task<List<Template.Models.DomainModels.Session>> GetSessionsByUserId(GetSessionsByUserIdRequest request);

        Task<List<Template.Models.DomainModels.Session>> GetSessionsByStartDate(GetSessionsByStartDateRequest request);
    }
}
