using System.Collections.Generic;
using System.Threading.Tasks;
using Template.Infrastructure.Repositories.SessionRepo.Models;
using Template.Models.DomainModels;

namespace Template.Infrastructure.Repositories.SessionRepo.Contracts
{
    public interface ISessionRepo
    {
        Task AddUserToSession(AddUserToSessionRequest request);

        Task CreateSession(CreateSessionRequest request);

        Task<Session> GetSessionByGuid(GetSessionByGuidRequest request);

        Task DeleteSession(DeleteSessionRequest request);

        Task<List<Session>> GetSessionsByUserId(GetSessionsByUserIdRequest request);
    }
}
