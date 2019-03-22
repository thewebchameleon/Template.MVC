using System.Collections.Generic;
using System.Threading.Tasks;
using Template.Infrastructure.Repositories.SessionRepo.Models;
using Template.Models.DomainModels;

namespace Template.Infrastructure.Repositories.SessionRepo.Contracts
{
    public interface ISessionRepo
    {
        Task<SessionEntity> AddUserToSession(AddUserToSessionRequest request);

        Task<SessionEntity> CreateSession(CreateSessionRequest request);

        Task<List<SessionEntity>> GetSessionsByUserId(GetSessionsByUserIdRequest request);

        Task<List<SessionEntity>> GetSessionsByStartDate(GetSessionsByStartDateRequest request);

        Task<List<SessionEntity>> GetSessionsByDate(GetSessionsByDateRequest request);

        Task<int> CreateSessionEvent(CreateSessionEventRequest request);

        Task UpdateSessionEvent(UpdateSessionEventRequest request);

        Task<List<SessionEventEntity>> GetSessionEvents();

        Task<List<SessionLogEventEntity>> GetSessionLogEventsBySessionId(GetSessionLogEventsBySessionIdRequest request);

        Task<List<SessionLogEntity>> GetSessionLogsBySessionId(GetSessionLogsBySessionIdRequest request);

        Task<SessionEntity> GetSessionById(GetSessionByIdRequest request);

        Task<int> CreateSessionLog(CreateSessionLogRequest request);

        Task<int> CreateSessionLogEvent(CreateSessionLogEventRequest request);
    }
}
