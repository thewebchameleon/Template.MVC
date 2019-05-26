using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using System.Threading.Tasks;
using Template.Common.Extensions;
using Template.Infrastructure.Cache.Contracts;
using Template.Infrastructure.Configuration;
using Template.Infrastructure.Email;
using Template.Infrastructure.Email.Contracts;
using Template.Infrastructure.UnitOfWork.Contracts;
using Template.Models;
using Template.Models.DomainModels;
using Template.Models.EmailTemplates;
using Template.Models.ServiceModels.Email;
using Template.Services.Contracts;

namespace Template.Services
{
    public class EmailService : IEmailService
    {
        #region Instance Fields

        private readonly IEmailProvider _emailProvider;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IApplicationCache _cache;
        private readonly IUnitOfWorkFactory _uowFactory;

        #endregion

        #region Constructors

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

        #endregion

        #region Public Methods

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
            var baseUrl = _httpContextAccessor.HttpContext.Request.GetBaseUrl();

            var templates = await _cache.EmailTemplates();
            var templateEntity = templates.FirstOrDefault(t => t.Key == EmailTemplateKeys.AccountActivation);

            var template = new AccountActivationTemplate(templateEntity.Body)
            {
                ActivationUrl = $"{baseUrl}/activate-account?token={activationToken}",
                ApplicationUrl = baseUrl
            };

            await _emailProvider.Send(new Infrastructure.Email.Models.SendRequest()
            {
                FromAddress = configuration.System_From_Email_Address,
                ToAddress = user.Email_Address,
                Subject = template.Subject,
                Body = template.GetHTMLContent()
            });
        }

        public async Task SendResetPassword(SendResetPasswordRequest request)
        {
            var resetToken = string.Empty;

            UserEntity user;
            using (var uow = _uowFactory.GetUnitOfWork())
            {
                user = await uow.UserRepo.GetUserById(new Infrastructure.Repositories.UserRepo.Models.GetUserByIdRequest()
                {
                    Id = request.UserId
                });

                resetToken = GenerateUniqueUserToken(uow);

                await uow.UserRepo.CreateUserToken(new Infrastructure.Repositories.UserRepo.Models.CreateUserTokenRequest()
                {
                    User_Id = request.UserId,
                    Token = new Guid(resetToken),
                    Type_Id = (int)TokenTypeEnum.ResetPassword,
                    Created_By = ApplicationConstants.SystemUserId,
                });
                uow.Commit();
            }

            var configuration = await _cache.Configuration();
            var baseUrl = _httpContextAccessor.HttpContext.Request.GetBaseUrl();

            var templates = await _cache.EmailTemplates();
            var templateEntity = templates.FirstOrDefault(t => t.Key == EmailTemplateKeys.ForgotPassword);

            var template = new ForgotPasswordTemplate(templateEntity.Body)
            {
                ResetPasswordUrl = $"{baseUrl}/reset-password?token={resetToken}",
                ApplicationUrl = baseUrl
            };

            await _emailProvider.Send(new Infrastructure.Email.Models.SendRequest()
            {
                FromAddress = configuration.System_From_Email_Address,
                ToAddress = user.Email_Address,
                Subject = template.Subject,
                Body = template.GetHTMLContent()
            });
        }

        public async Task SendContactMessage(SendContactMessageRequest request)
        {
            var configuration = await _cache.Configuration();
            var baseUrl = _httpContextAccessor.HttpContext.Request.GetBaseUrl();

            var templates = await _cache.EmailTemplates();
            var templateEntity = templates.FirstOrDefault(t => t.Key == EmailTemplateKeys.ContactMessage);

            var template = new ContactMessageTemplate(templateEntity.Body)
            {
                Name = request.Name,
                Message = request.Message,
                ApplicationUrl = baseUrl
            };

            await _emailProvider.Send(new Infrastructure.Email.Models.SendRequest()
            {
                FromAddress = request.EmailAddress,
                ToAddress = configuration.Contact_Email_Address,
                Subject = template.Subject,
                Body = template.GetHTMLContent()
            });
        }

        #endregion

        #region Private Methods

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

        #endregion
    }
}
