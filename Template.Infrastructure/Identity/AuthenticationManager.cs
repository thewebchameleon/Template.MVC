using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Template.Infrastructure.Configuration;
using Template.Infrastructure.Session;
using Template.Infrastructure.Session.Contracts;
using Template.Infrastructure.UnitOfWork.Contracts;
using Template.Models.DomainModels;

namespace Template.Infrastructure.Identity
{
    public class AuthenticationManager : SignInManager<UserEntity>
    {
        private readonly IUnitOfWorkFactory _uowFactory;
        private readonly ISessionProvider _sessionProvider;

        public AuthenticationManager(
            UserManager<UserEntity> userManager,
            IHttpContextAccessor contextAccessor,
            IUserClaimsPrincipalFactory<UserEntity> claimsFactory,
            IOptions<IdentityOptions> optionsAccessor,
            ILogger<SignInManager<UserEntity>> logger,
            IUnitOfWorkFactory uowFactory,
            ISessionProvider sessionProvider,
            IAuthenticationSchemeProvider schemes)
            : base(userManager, contextAccessor, claimsFactory, optionsAccessor, logger, schemes)
        {
            _uowFactory = uowFactory;
            _sessionProvider = sessionProvider;
        }

        public async override Task<ClaimsPrincipal> CreateUserPrincipalAsync(UserEntity user)
        {
            var principal = await base.CreateUserPrincipalAsync(user);

            if (!_sessionProvider.TryGet(SessionConstants.SessionEntity, out Models.DomainModels.SessionEntity session))
            {
                throw new Exception("Session has not been initialised");
            }

            using (var uow = _uowFactory.GetUnitOfWork())
            {
                session = await uow.SessionRepo.AddUserToSession(new Repositories.SessionRepo.Models.AddUserToSessionRequest()
                {
                    Id = session.Id,
                    User_Id = user.Id,
                    Updated_By = ApplicationConstants.SystemUserId
                });
                uow.Commit();

                _sessionProvider.Set(SessionConstants.SessionEntity, session);
            }

            return principal;
        }

        public async override Task<bool> CanSignInAsync(UserEntity user)
        {
            return user.Is_Enabled && !user.Is_Deleted;
        }
    }
}
