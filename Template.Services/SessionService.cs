using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
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

        #endregion

        #region Constructor

        public SessionService(
            ILogger<SessionService> logger,
            ISessionProvider sessionProvider,
            IUnitOfWorkFactory uowFactory)
        {
            _logger = logger;
            _uowFactory = uowFactory;
            _sessionProvider = sessionProvider;
        }

        #endregion

        #region Public Methods

        public async Task<GetAuthenticatedSessionResponse> GetAuthenticatedSession()
        {
            var getSessionResponse = await GetSession();
            if (getSessionResponse.User == null)
            {
                throw new Exception("Session is not authenticated");
            }

            return getSessionResponse as GetAuthenticatedSessionResponse;
        }

        public async Task<GetSessionResponse> GetSession()
        {
            var response = new GetSessionResponse();

            // get or create a new session
            if (!_sessionProvider.TryGet(SessionConstants.SessionEntity, out Session session))
            {
                using (var uow = _uowFactory.GetUnitOfWork())
                {
                    session = await uow.SessionRepo.CreateSession(new Infrastructure.Repositories.SessionRepo.Models.CreateSessionRequest()
                    {
                        Created_By = ApplicationConstants.SystemUserId
                    });
                    uow.Commit();

                    _sessionProvider.Set(SessionConstants.SessionEntity, session);
                }
            }
            response.Id = session.Id;

            // get / rehydrate user from session if authenticated
            if (!_sessionProvider.TryGet(SessionConstants.UserEntity, out User user)
                && session.User_Id.HasValue)
            {
                using (var uow = _uowFactory.GetUnitOfWork())
                {
                    user = await uow.UserRepo.GetUserById(new Infrastructure.Repositories.UserRepo.Models.GetUserByIdRequest()
                    {
                        User_Id = session.User_Id.Value
                    });
                }
            }
            response.User = user;

            return response;
        }

        #endregion
    }
}
