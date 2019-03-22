using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using Template.Infrastructure.Cache.Contracts;
using Template.Infrastructure.Configuration;
using Template.Infrastructure.Session;
using Template.Infrastructure.Session.Contracts;
using Template.Infrastructure.UnitOfWork.Contracts;
using Template.Models.DomainModels;
using Template.Models.ServiceModels.Session;
using Template.Services.Contracts;

namespace Template.Services
{
    public class SessionService : ISessionService
    {
        #region Instance Fields

        private readonly ILogger<SessionService> _logger;

        private readonly ISessionProvider _sessionProvider;
        private readonly IUnitOfWorkFactory _uowFactory;
        private readonly IEntityCache _entityCache;

        private readonly IHttpContextAccessor _httpContextAccessor;

        #endregion

        #region Constructor

        public SessionService(
            ILogger<SessionService> logger,
            ISessionProvider sessionProvider,
            IUnitOfWorkFactory uowFactory,
            IEntityCache entityCache,
            IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _uowFactory = uowFactory;
            _sessionProvider = sessionProvider;
            _httpContextAccessor = httpContextAccessor;
            _entityCache = entityCache;
        }

        #endregion

        #region Public Methods

        public async Task<GetSessionResponse> GetAuthenticatedSession()
        {
            var getSessionResponse = await GetSession();
            if (getSessionResponse.User == null)
            {
                throw new Exception("Session is not authenticated");
            }

            return getSessionResponse;
        }

        public async Task<GetSessionResponse> GetSession()
        {
            var response = new GetSessionResponse();

            // get or create a new session
            var session = await _sessionProvider.Get<SessionEntity>(SessionConstants.SessionEntity);
            if (session == null)
            {
                await _httpContextAccessor.HttpContext.SignOutAsync(); // flush any authenticated cookies in the event the application restarts

                using (var uow = _uowFactory.GetUnitOfWork())
                {
                    session = await uow.SessionRepo.CreateSession(new Infrastructure.Repositories.SessionRepo.Models.CreateSessionRequest()
                    {
                        Created_By = ApplicationConstants.SystemUserId
                    });
                    uow.Commit();

                    await _sessionProvider.Set(SessionConstants.SessionEntity, session);
                }
            }
            response.Id = session.Id;
            response.SessionLogId = await _sessionProvider.Get<int>(SessionConstants.SessionLogId);

            // get or hydrate user from session
            var user = await _sessionProvider.Get<UserEntity>(SessionConstants.UserEntity);
            if (user == null
                && session.User_Id.HasValue)
            {
                using (var uow = _uowFactory.GetUnitOfWork())
                {
                    user = await uow.UserRepo.GetUserById(new Infrastructure.Repositories.UserRepo.Models.GetUserByIdRequest()
                    {
                        User_Id = session.User_Id.Value
                    });
                    uow.Commit();

                    await _sessionProvider.Set(SessionConstants.UserEntity, user);
                }
            }
            response.User = user;

            return response;
        }

        public async Task RehydrateSession()
        {
            await _sessionProvider.Remove(SessionConstants.UserEntity);
        }

        public async Task WriteSessionLogEvent(CreateSessionLogEventRequest request)
        {
            var session = await GetSession();

            var events = await _entityCache.SessionEvents();
            var eventItem = events.FirstOrDefault(e => e.Key == request.EventKey);

            if (eventItem == null)
            {
                // todo: rather log this than throw an exception
                throw new Exception($"Could not find session log event with key {request.EventKey}");
            }

            using (var uow = _uowFactory.GetUnitOfWork())
            {
                await uow.SessionRepo.CreateSessionLogEvent(new Infrastructure.Repositories.SessionRepo.Models.CreateSessionLogEventRequest()
                {
                    Session_Log_Id = session.SessionLogId,
                    Event_Id = eventItem.Id,
                    Message = request.Message,
                    Created_By = ApplicationConstants.SystemUserId
                });
                uow.Commit();

                await _sessionProvider.Set(SessionConstants.SessionEntity, session);
            }
        }

        #endregion
    }
}
