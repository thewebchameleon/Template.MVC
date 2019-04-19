using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;
using Template.Infrastructure.Cache.Contracts;
using Template.Infrastructure.Configuration;
using Template.Infrastructure.Email.Contracts;
using Template.Infrastructure.UnitOfWork.Contracts;
using Template.Models;
using Template.Models.DomainModels;
using Template.Models.ServiceModels.Email;
using Template.Services.Contracts;

namespace Template.Services
{
    public class EmailService : IEmailService
    {
        private readonly IEmailProvider _emailProvider;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IApplicationCache _cache;
        private readonly IUnitOfWorkFactory _uowFactory;

        public EmailService(
            IEmailProvider emailProvider,
            IHttpContextAccessor httpContextAccessor,
            IApplicationCache cache,
            IUnitOfWorkFactory uowFactory)
        {
            _emailProvider = emailProvider;
            _httpContextAccessor = httpContextAccessor;
            _cache = cache;
            _uowFactory = uowFactory;
        }

        public async Task SendAccountActivation(SendAccountActivationRequest request)
        {
            var activationToken = string.Empty;

            UserEntity user;
            using (var uow = _uowFactory.GetUnitOfWork())
            {
                user = await uow.UserRepo.GetUserById(new Infrastructure.Repositories.UserRepo.Models.GetUserByIdRequest()
                {
                    Id = request.UserId
                });

                activationToken = GenerateUniqueUserToken(uow);

                await uow.UserRepo.CreateUserToken(new Infrastructure.Repositories.UserRepo.Models.CreateUserTokenRequest()
                {
                    User_Id = request.UserId,
                    Token = new Guid(activationToken),
                    Type_Id = (int)TokenTypeEnum.AccountActivation,
                    Created_By = ApplicationConstants.SystemUserId,
                });
                uow.Commit();
            }

            var configuration = await _cache.Configuration();

            var httpRequest = _httpContextAccessor.HttpContext.Request;
            var host = httpRequest.Host.ToUriComponent();
            var pathBase = httpRequest.PathBase.ToUriComponent();

            var baseUrl = $"{httpRequest.Scheme}://{host}{pathBase}";

            var tokenLink = $"{baseUrl}/Account/Activate?token={activationToken}";
            await _emailProvider.Send(new Infrastructure.Email.Models.SendRequest()
            {
                FromAddress = configuration.System_From_Email_Address,
                ToAddress = user.Email_Address,
                Subject = "Please activate your account",
                Body = $@"
                    <html>
                        <h1>Please activate your account by clicking the link below</h1>
                        <a href='{tokenLink}'>Activate</a>
                    </html>
                    "
            });
        }

        public async Task SendResetPassword(SendResetPasswordRequest request)
        {
            var activationToken = string.Empty;

            UserEntity user;
            using (var uow = _uowFactory.GetUnitOfWork())
            {
                user = await uow.UserRepo.GetUserById(new Infrastructure.Repositories.UserRepo.Models.GetUserByIdRequest()
                {
                    Id = request.UserId
                });

                activationToken = GenerateUniqueUserToken(uow);

                await uow.UserRepo.CreateUserToken(new Infrastructure.Repositories.UserRepo.Models.CreateUserTokenRequest()
                {
                    User_Id = request.UserId,
                    Token = new Guid(activationToken),
                    Type_Id = (int)TokenTypeEnum.ResetPassword,
                    Created_By = ApplicationConstants.SystemUserId,
                });
                uow.Commit();
            }

            var configuration = await _cache.Configuration();

            var httpRequest = _httpContextAccessor.HttpContext.Request;
            var host = httpRequest.Host.ToUriComponent();
            var pathBase = httpRequest.PathBase.ToUriComponent();

            var baseUrl = $"{httpRequest.Scheme}://{host}{pathBase}";

            var tokenLink = $"{baseUrl}/Account/ResetPassword?token={activationToken}";
            await _emailProvider.Send(new Infrastructure.Email.Models.SendRequest()
            {
                FromAddress = configuration.System_From_Email_Address,
                ToAddress = user.Email_Address,
                Subject = "Forgot password request",
                Body = $@"
                    <html>
                        <h1>A forgot password request has been made to your account</h1>
                        <h2>Please reset your password by clicking the link below</h2>
                        <a href='{tokenLink}'>Reset Password</a>
                    </html>
                    "
            });
        }

        private string GenerateUniqueUserToken(IUnitOfWork uow)
        {
            var generatedCode = GenerateGuid();

            while (CheckUserTokenExists(uow, generatedCode))
            {
                generatedCode = GenerateGuid();
            }
            return generatedCode;
        }

        private string GenerateGuid()
        {
            return Guid.NewGuid().ToString("N");
        }

        private bool CheckUserTokenExists(IUnitOfWork uow, string token)
        {
            var tokenResult = uow.UserRepo.GetUserTokenByGuid(new Infrastructure.Repositories.UserRepo.Models.GetUserTokenByGuidRequest()
            {
                Guid = new Guid(token)
            });
            tokenResult.Wait();
            return tokenResult.Result != null;
        }
    }
}
