using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Template.Common.Notifications;
using Template.Infrastructure.Cache;
using Template.Infrastructure.Cache.Contracts;
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
        private IEnumerable<object> newClaims;

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

        public async Task<EnableUserResponse> EnableUser(EnableUserRequest request, int userId)
        {
            var response = new EnableUserResponse();

            var users = await _entityCache.Users();
            var user = users.FirstOrDefault(u => u.Id == request.UserId);

            user.Is_Enabled = true;
            user.Updated_By = userId;

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

        public async Task<DisableUserResponse> DisableUser(DisableUserRequest request, int userId)
        {
            var response = new DisableUserResponse();

            var users = await _entityCache.Users();
            var user = users.FirstOrDefault(u => u.Id == request.UserId);

            user.Is_Enabled = false;
            user.Updated_By = userId;

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

            var userRoles = await _entityCache.UserRoles();
            var userClaims = await _entityCache.UserClaims();
            var roles = await _entityCache.Roles();
            var claims = await _entityCache.Claims();
            var users = await _entityCache.Users();

            var user = users.FirstOrDefault(u => u.Id == request.UserId);
            var usersRoles = userRoles.Where(ur => ur.User_Id == request.UserId).Select(ur => ur.Role_Id);
            var usersClaims = userClaims.Where(uc => uc.User_Id == request.UserId).Select(uc => uc.Claim_Id);

            response.Roles = roles.Where(r => usersRoles.Contains(r.Id)).ToList();
            response.Claims = claims.Where(c => usersClaims.Contains(c.Id)).ToList();

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

        public async Task<CreateUserResponse> CreateUser(CreateUserRequest request, int userId)
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
                Created_By = userId,
                Updated_By = userId
            };

            await _userManager.CreateAsync(user, request.Password);
            _entityCache.Remove(CacheConstants.Users);

            if (request.Roles.Any())
            {
                await CreateOrDeleteUserRoles(request.Roles, user.Id, userId);
                _entityCache.Remove(CacheConstants.UserRoles);
            }
            if (request.Claims.Any())
            {
                await CreateOrDeleteUserClaims(request.Claims, user.Id, userId);
                _entityCache.Remove(CacheConstants.UserClaims);
            }

            await _emailService.SendAccountActivationEmail(new SendAccountActivationEmailRequest()
            {
                UserID = user.Id
            });

            response.Notifications.Add($"User {request.Username} has been created", NotificationTypeEnum.Success);
            return response;
        }

        public async Task<UpdateUserResponse> UpdateUser(UpdateUserRequest request, int userId)
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
            user.Updated_By = userId;

            await CreateOrDeleteUserRoles(request.Roles, request.UserId, userId);
            await CreateOrDeleteUserClaims(request.Claims, request.UserId, userId);

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

        private async Task CreateOrDeleteUserRoles(List<int> newRoles, int userId, int loggedInUserId)
        {
            var userRoles = await _entityCache.UserRoles();
            var existingRoles = userRoles.Where(ur => ur.User_Id == userId).Select(ur => ur.Role_Id);

            var rolesToBeDeleted = existingRoles.Where(ur => !newRoles.Contains(ur));
            var rolesToBeCreated = newRoles.Where(ur => !existingRoles.Contains(ur));

            if (rolesToBeDeleted.Any() || rolesToBeCreated.Any())
            {
                using (var uow = _uowFactory.GetUnitOfWork())
                {
                    foreach (var roleId in rolesToBeCreated)
                    {
                        await uow.UserRepo.CreateUserRole(new Infrastructure.Repositories.UserRepo.Models.CreateUserRoleRequest()
                        {
                            User_Id = userId,
                            Role_Id = roleId,
                            Created_By = loggedInUserId
                        });

                    }

                    foreach (var roleId in rolesToBeDeleted)
                    {
                        await uow.UserRepo.DeleteUserRole(new Infrastructure.Repositories.UserRepo.Models.DeleteUserRoleRequest()
                        {
                            User_Id = userId,
                            Role_Id = roleId,
                            Updated_By = loggedInUserId
                        });

                    }
                    uow.Commit();
                }
                _entityCache.Remove(CacheConstants.UserRoles);
            }
        }

        private async Task CreateOrDeleteUserClaims(List<int> newClaims, int userId, int loggedInUserId)
        {
            var userClaims = await _entityCache.UserClaims();
            var existingClaims = userClaims.Where(ur => ur.User_Id == userId).Select(ur => ur.Claim_Id);

            var claimsToBeDeleted = existingClaims.Where(ur => !newClaims.Contains(ur));
            var claimsToBeCreated = newClaims.Where(ur => !existingClaims.Contains(ur));

            if (claimsToBeDeleted.Any() || claimsToBeCreated.Any())
            {
                using (var uow = _uowFactory.GetUnitOfWork())
                {
                    foreach (var claimId in claimsToBeCreated)
                    {
                        await uow.UserRepo.CreateUserClaim(new Infrastructure.Repositories.UserRepo.Models.CreateUserClaimRequest()
                        {
                            User_Id = userId,
                            Claim_Id = claimId,
                            Created_By = loggedInUserId
                        });

                    }

                    foreach (var roleId in claimsToBeDeleted)
                    {
                        await uow.UserRepo.DeleteUserClaim(new Infrastructure.Repositories.UserRepo.Models.DeleteUserClaimRequest()
                        {
                            User_Id = userId,
                            Claim_Id = roleId,
                            Updated_By = loggedInUserId
                        });

                    }
                    uow.Commit();
                }
                _entityCache.Remove(CacheConstants.UserClaims);
            }
        }

        #endregion
    }
}
