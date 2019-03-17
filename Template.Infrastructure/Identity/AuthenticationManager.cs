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
    public class AuthenticationManager : SignInManager<User>
    {
        private readonly IUnitOfWorkFactory _uowFactory;
        private readonly ISessionProvider _sessionProvider;

        public AuthenticationManager(
            UserManager<User> userManager,
            IHttpContextAccessor contextAccessor,
            IUserClaimsPrincipalFactory<User> claimsFactory,
            IOptions<IdentityOptions> optionsAccessor,
            ILogger<SignInManager<User>> logger,
            IUnitOfWorkFactory uowFactory,
            ISessionProvider sessionProvider,
            IAuthenticationSchemeProvider schemes)
            : base(userManager, contextAccessor, claimsFactory, optionsAccessor, logger, schemes)
        {
            _uowFactory = uowFactory;
            _sessionProvider = sessionProvider;
        }

        public async override Task<ClaimsPrincipal> CreateUserPrincipalAsync(User user)
        {
            var principal = await base.CreateUserPrincipalAsync(user);

            if (!_sessionProvider.TryGet(SessionConstants.SessionEntity, out Models.DomainModels.Session session))
            {
                throw new Exception("Session has not been initialised");
            }

            using (var uow = _uowFactory.GetUnitOfWork())
            {
                await uow.SessionRepo.AddUserToSession(new Repositories.SessionRepo.Models.AddUserToSessionRequest()
                {
                    Id = session.Id,
                    User_Id = user.Id,
                    Updated_By = ApplicationConstants.SystemUserId
                });
                uow.Commit();
            }

            return principal;
        }

        public async override Task<bool> CanSignInAsync(User user)
        {
            return user.Is_Enabled && !user.Is_Deleted;
        }
    }
}
