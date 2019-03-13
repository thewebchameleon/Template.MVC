using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using Template.Common.Notifications;
using Template.Infrastructure.Cache;
using Template.Infrastructure.Cache.Contracts;
using Template.Infrastructure.Configuration;
using Template.Infrastructure.UnitOfWork.Contracts;
using Template.Models.DomainModels;
using Template.Models.ServiceModels;
using Template.Models.ServiceModels.Admin;
using Template.Services.Contracts;

namespace Template.Services
{
    public class AdminService : IAdminService
    {
        #region Instance Fields

        private readonly ILogger<AdminService> _logger;

        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly SignInManager<User> _signInManager;

        private readonly IEmailService _emailService;
        private readonly IAccountService _accountService;

        private readonly IUnitOfWorkFactory _uowFactory;
        private readonly IEntityCache _entityCache;

        #endregion

        #region Constructor

        public AdminService(
            ILogger<AdminService> logger,
            UserManager<User> userManager,
            RoleManager<Role> roleManager,
            SignInManager<User> signInManager,
            IEmailService emailService,
            IAccountService accountService,
            IUnitOfWorkFactory uowFactory,
            IEntityCache entityCache)
        {
            _logger = logger;

            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;

            _uowFactory = uowFactory;

            _entityCache = entityCache;
            _emailService = emailService;
            _accountService = accountService;
        }

        #endregion

        #region Public Methods

        public async Task<EnableUserResponse> EnableUser(EnableUserRequest request)
        {
            var response = new EnableUserResponse();

            var users = await _entityCache.Users();
            var user = users.FirstOrDefault(u => u.Id == request.UserId);

            user.Is_Enabled = true;

            var updateResponse = await _userManager.UpdateAsync(user);
            if (!updateResponse.Succeeded)
            {
                response.Notifications.AddErrors(updateResponse.Errors.Select(e => e.Description).ToList());
                return response;
            }

            _entityCache.Remove(CacheConstants.Users);

            response.Notifications.Add($"User {user.Username} has been enabled", NotificationTypeEnum.Success);
            return response;
        }

        public async Task<DisableUserResponse> DisableUser(DisableUserRequest request)
        {
            var response = new DisableUserResponse();

            var users = await _entityCache.Users();
            var user = users.FirstOrDefault(u => u.Id == request.UserId);

            user.Is_Enabled = false;

            var updateResponse = await _userManager.UpdateAsync(user);
            if (!updateResponse.Succeeded)
            {
                response.Notifications.AddErrors(updateResponse.Errors.Select(e => e.Description).ToList());
                return response;
            }

            _entityCache.Remove(CacheConstants.Users);

            response.Notifications.Add($"User {user.Username} has been disabled", NotificationTypeEnum.Success);
            return response;
        }

        public async Task<GetUserResponse> GetUser(GetUserRequest request)
        {
            var response = new GetUserResponse();

            var users = await _entityCache.Users();
            var user = users.FirstOrDefault(u => u.Id == request.UserId);

            if (user == null)
            {
                response.Notifications.AddError($"Could not find user with Id {request.UserId}");
                return response;
            }
            response.User = user;
            return response;
        }

        public async Task<GetUserManagementResponse> GetUserManagement(GetUserManagementRequest request)
        {
            var response = new GetUserManagementResponse();

            response.Users = await _entityCache.Users();

            return response;
        }

        public async Task<CreateUserResponse> CreateUser(CreateUserRequest request)
        {
            var response = new CreateUserResponse();
            var username = request.EmailAddress;

            var duplicateResponse = await _accountService.DuplicateCheck(new DuplicateCheckRequest()
            {
                EmailAddress = request.EmailAddress,
                Username = username
            });

            if (duplicateResponse.Notifications.HasErrors)
            {
                response.Notifications.Add(duplicateResponse.Notifications);
                return response;
            }

            var user = new User()
            {
                Username = username,
                First_Name = request.FirstName,
                Last_Name = request.LastName,
                Mobile_Number = request.MobileNumber,
                Email_Address = request.EmailAddress,
                Created_By = ApplicationConstants.SystemUserId,
                Created_Date = DateTime.Now,
                Updated_By = ApplicationConstants.SystemUserId,
                Updated_Date = DateTime.Now,
            };

            await _userManager.CreateAsync(user, request.Password);
            await _emailService.SendAccountActivationEmail(new SendAccountActivationEmailRequest()
            {
                UserID = user.Id
            });

            _entityCache.Remove(CacheConstants.Users);
            _entityCache.Remove(CacheConstants.UserRoles);

            response.Notifications.Add($"User {request.Username} has been created", NotificationTypeEnum.Success);
            return response;
        }

        public async Task<UpdateUserResponse> UpdateUser(UpdateUserRequest request)
        {
            var response = new UpdateUserResponse();

            var users = await _entityCache.Users();
            var user = users.FirstOrDefault(u => u.Id == request.UserId);

            user.Username = request.Username;
            user.Email_Address = request.EmailAddress;
            user.First_Name = request.FirstName;
            user.Last_Name = request.LastName;
            user.Mobile_Number = request.MobileNumber;
            user.Registration_Confirmed = request.RegistrationConfirmed;
            user.Lockout_End = request.Lockout_End;
            user.Is_Locked_Out = request.Is_Locked_Out;

            if (!string.IsNullOrEmpty(request.Password))
            {
                var resetPasswordToken = await _userManager.GeneratePasswordResetTokenAsync(user);
                var updatePasswordResponse = await _userManager.ResetPasswordAsync(user, resetPasswordToken, request.Password);
                if (!updatePasswordResponse.Succeeded)
                {
                    response.Notifications.AddErrors(updatePasswordResponse.Errors.Select(e => e.Description).ToList());
                    return response;
                }
            }
            var updateResponse = await _userManager.UpdateAsync(user);
            if (!updateResponse.Succeeded)
            {
                response.Notifications.AddErrors(updateResponse.Errors.Select(e => e.Description).ToList());
                return response;
            }

            _entityCache.Remove(CacheConstants.Users);

            response.Notifications.Add($"User {user.Username} has been updated", NotificationTypeEnum.Success);
            return response;
        }

        #endregion
    }
}
