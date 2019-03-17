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

        #region User Management

        public async Task<GetUserManagementResponse> GetUserManagement()
        {
            var response = new GetUserManagementResponse();

            using (var uow = _uowFactory.GetUnitOfWork())
            {
                response.Users = await uow.UserRepo.GetUsers();
                uow.Commit();

                return response;
            }
        }

        public async Task<EnableUserResponse> EnableUser(EnableUserRequest request, int userId)
        {
            var response = new EnableUserResponse();

            User user;
            using (var uow = _uowFactory.GetUnitOfWork())
            {
                user = await uow.UserRepo.GetUserById(new Infrastructure.Repositories.UserRepo.Models.GetUserByIdRequest()
                {
                    User_Id = request.UserId
                });
                uow.Commit();
            }
            user.Is_Enabled = true;
            user.Updated_By = userId;

            var updateResponse = await _userManager.UpdateAsync(user);
            if (!updateResponse.Succeeded)
            {
                response.Notifications.AddErrors(updateResponse.Errors.Select(e => e.Description).ToList());
                return response;
            }

            _entityCache.Remove(CacheConstants.Users);

            response.Notifications.Add($"User '{user.Username}' has been enabled", NotificationTypeEnum.Success);
            return response;
        }

        public async Task<DisableUserResponse> DisableUser(DisableUserRequest request, int userId)
        {
            var response = new DisableUserResponse();

            User user;
            using (var uow = _uowFactory.GetUnitOfWork())
            {
                user = await uow.UserRepo.GetUserById(new Infrastructure.Repositories.UserRepo.Models.GetUserByIdRequest()
                {
                    User_Id = request.Id
                });
                uow.Commit();
            }

            user.Is_Enabled = false;
            user.Updated_By = userId;

            var updateResponse = await _userManager.UpdateAsync(user);
            if (!updateResponse.Succeeded)
            {
                response.Notifications.AddErrors(updateResponse.Errors.Select(e => e.Description).ToList());
                return response;
            }

            _entityCache.Remove(CacheConstants.Users);

            response.Notifications.Add($"User '{user.Username}' has been disabled", NotificationTypeEnum.Success);
            return response;
        }

        public async Task<GetUserResponse> GetUser(GetUserRequest request)
        {
            var response = new GetUserResponse();

            var userRoles = await _entityCache.UserRoles();
            var roles = await _entityCache.Roles();
            var claims = await _entityCache.Claims();

            using (var uow = _uowFactory.GetUnitOfWork())
            {
                var user = await uow.UserRepo.GetUserById(new Infrastructure.Repositories.UserRepo.Models.GetUserByIdRequest()
                {
                    User_Id = request.UserId
                });
                uow.Commit();

                var usersRoles = userRoles.Where(ur => ur.User_Id == request.UserId).Select(ur => ur.Role_Id);

                response.Roles = roles.Where(r => usersRoles.Contains(r.Id)).ToList();

                if (user == null)
                {
                    response.Notifications.AddError($"Could not find user with Id {request.UserId}");
                    return response;
                }
                response.User = user;
                return response;
            }
        }

        public async Task<CreateUserResponse> CreateUser(CreateUserRequest request, int userId)
        {
            var response = new CreateUserResponse();
            var username = request.EmailAddress;

            var duplicateResponse = await _accountService.DuplicateUserCheck(new DuplicateUserCheckRequest()
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
                Updated_By = userId,
                Is_Enabled = true
            };

            await _userManager.CreateAsync(user, request.Password);
            _entityCache.Remove(CacheConstants.Users);

            await CreateOrDeleteUserRoles(request.RoleIds, user.Id, userId);

            response.Notifications.Add($"User {request.Username} has been created", NotificationTypeEnum.Success);
            return response;
        }

        public async Task<UpdateUserResponse> UpdateUser(UpdateUserRequest request, int userId)
        {
            var response = new UpdateUserResponse();

            User user;
            using (var uow = _uowFactory.GetUnitOfWork())
            {
                user = await uow.UserRepo.GetUserById(new Infrastructure.Repositories.UserRepo.Models.GetUserByIdRequest()
                {
                    User_Id = request.UserId
                });
                uow.Commit();
            }

            user.Username = request.Username;
            user.Email_Address = request.EmailAddress;
            user.First_Name = request.FirstName;
            user.Last_Name = request.LastName;
            user.Mobile_Number = request.MobileNumber;
            user.Registration_Confirmed = request.RegistrationConfirmed;
            user.Lockout_End = request.Lockout_End;
            user.Is_Locked_Out = request.Is_Locked_Out;
            user.Updated_By = userId;

            await CreateOrDeleteUserRoles(request.RoleIds, request.UserId, userId);

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

        #endregion

        #region Role Management

        public async Task<GetRoleManagementResponse> GetRoleManagement()
        {
            var response = new GetRoleManagementResponse();

            response.Roles = await _entityCache.Roles();

            return response;
        }

        public async Task<EnableRoleResponse> EnableRole(EnableRoleRequest request, int userId)
        {
            var response = new EnableRoleResponse();

            var roles = await _entityCache.Roles();
            var role = roles.FirstOrDefault(u => u.Id == request.RoleId);

            role.Is_Enabled = true;
            role.Updated_By = userId;

            var updateResponse = await _roleManager.UpdateAsync(role);
            if (!updateResponse.Succeeded)
            {
                response.Notifications.AddErrors(updateResponse.Errors.Select(e => e.Description).ToList());
                return response;
            }

            _entityCache.Remove(CacheConstants.Roles);

            response.Notifications.Add($"Role '{role.Name}' has been enabled", NotificationTypeEnum.Success);
            return response;
        }

        public async Task<DisableRoleResponse> DisableRole(DisableRoleRequest request, int roleId)
        {
            var response = new DisableRoleResponse();

            var roles = await _entityCache.Roles();
            var role = roles.FirstOrDefault(u => u.Id == request.Id);

            role.Is_Enabled = false;
            role.Updated_By = roleId;

            var updateResponse = await _roleManager.UpdateAsync(role);
            if (!updateResponse.Succeeded)
            {
                response.Notifications.AddErrors(updateResponse.Errors.Select(e => e.Description).ToList());
                return response;
            }

            _entityCache.Remove(CacheConstants.Roles);

            response.Notifications.Add($"Role '{role.Name}' has been disabled", NotificationTypeEnum.Success);
            return response;
        }

        public async Task<GetRoleResponse> GetRole(GetRoleRequest request)
        {
            var response = new GetRoleResponse();

            var roles = await _entityCache.Roles();
            var claims = await _entityCache.Claims();
            var roleClaims = await _entityCache.RoleClaims();

            var role = roles.FirstOrDefault(r => r.Id == request.RoleId);
            if (role == null)
            {
                response.Notifications.AddError($"Could not find role with Id {request.RoleId}");
                return response;
            }

            var rolesClaims = roleClaims.Where(rc => rc.Role_Id == request.RoleId).Select(rc => rc.Claim_Id);

            response.Role = role;
            response.Claims = claims.Where(c => rolesClaims.Contains(c.Id)).ToList();

            response.Role = role;
            return response;
        }

        public async Task<CreateRoleResponse> CreateRole(CreateRoleRequest request, int userId)
        {
            var response = new CreateRoleResponse();

            var duplicateResponse = await _accountService.DuplicateRoleCheck(new DuplicateRoleCheckRequest()
            {
                Name = request.Name
            });

            if (duplicateResponse.Notifications.HasErrors)
            {
                response.Notifications.Add(duplicateResponse.Notifications);
                return response;
            }

            var role = new Role()
            {
                Name = request.Name,
                Description = request.Description,
                Created_By = userId,
                Updated_By = userId,
                Is_Enabled = true
            };

            await _roleManager.CreateAsync(role);
            _entityCache.Remove(CacheConstants.Roles);

            await CreateOrDeleteRoleClaims(request.ClaimIds, role.Id, userId);

            response.Notifications.Add($"Role '{request.Name}' has been created", NotificationTypeEnum.Success);
            return response;
        }

        public async Task<UpdateRoleResponse> UpdateRole(UpdateRoleRequest request, int userId)
        {
            var response = new UpdateRoleResponse();

            var roles = await _entityCache.Roles();
            var role = roles.FirstOrDefault(u => u.Id == request.RoleId);

            role.Name = request.Name;
            role.Description = request.Description;
            role.Updated_By = userId;

            await CreateOrDeleteRoleClaims(request.ClaimIds, request.RoleId, userId);

            var updateResponse = await _roleManager.UpdateAsync(role);
            if (!updateResponse.Succeeded)
            {
                response.Notifications.AddErrors(updateResponse.Errors.Select(e => e.Description).ToList());
                return response;
            }

            _entityCache.Remove(CacheConstants.Roles);

            response.Notifications.Add($"Role '{role.Name}' has been updated", NotificationTypeEnum.Success);
            return response;
        }

        private async Task CreateOrDeleteRoleClaims(List<int> newClaims, int roleId, int loggedInUserId)
        {
            var roleClaims = await _entityCache.RoleClaims();
            var existingClaims = roleClaims.Where(rc => rc.Role_Id == roleId).Select(rc => rc.Claim_Id);

            var claimsToBeDeleted = existingClaims.Where(ur => !newClaims.Contains(ur));
            var claimsToBeCreated = newClaims.Where(ur => !existingClaims.Contains(ur));

            if (claimsToBeDeleted.Any() || claimsToBeCreated.Any())
            {
                using (var uow = _uowFactory.GetUnitOfWork())
                {
                    foreach (var claimId in claimsToBeCreated)
                    {
                        await uow.UserRepo.CreateRoleClaim(new Infrastructure.Repositories.UserRepo.Models.CreateRoleClaimRequest()
                        {
                            Role_Id = roleId,
                            Claim_Id = claimId,
                            Created_By = loggedInUserId
                        });

                    }

                    foreach (var claimId in claimsToBeDeleted)
                    {
                        await uow.UserRepo.DeleteRoleClaim(new Infrastructure.Repositories.UserRepo.Models.DeleteRoleClaimRequest()
                        {
                            Role_Id = roleId,
                            Claim_Id = claimId,
                            Updated_By = loggedInUserId
                        });

                    }
                    uow.Commit();
                }
                _entityCache.Remove(CacheConstants.RoleClaims);
            }
        }

        #endregion

        #region Configuration Management

        public async Task<GetConfigurationManagementResponse> GetConfigurationManagement()
        {
            var response = new GetConfigurationManagementResponse();

            response.ConfigurationItems = await _entityCache.ConfigurationItems();

            return response;
        }

        public async Task<GetConfigurationItemResponse> GetConfigurationItem(GetConfigurationItemRequest request)
        {
            var response = new GetConfigurationItemResponse();

            var configItems = await _entityCache.ConfigurationItems();
            var configItem = configItems.FirstOrDefault(c => c.Id == request.Id);

            response.ConfigurationItem = configItem;

            return response;
        }

        public async Task<UpdateConfigurationItemResponse> UpdateConfigurationItem(UpdateConfigurationItemRequest request, int userId)
        {
            var response = new UpdateConfigurationItemResponse();

            using (var uow = _uowFactory.GetUnitOfWork())
            {
                await uow.ConfigurationRepo.UpdateConfigurationItem(new Infrastructure.Repositories.ConfigurationRepo.Models.UpdateConfigurationItemRequest()
                {
                    Id = request.Id,
                    Description = request.Description,
                    Boolean_Value = request.BooleanValue,
                    DateTime_Value = request.DateTimeValue,
                    Decimal_Value = request.DecimalValue,
                    Int_Value = request.IntValue,
                    Money_Value = request.MoneyValue,
                    String_Value = request.StringValue,
                    Updated_By = userId
                });
                uow.Commit();
            }

            _entityCache.Remove(CacheConstants.ConfigurationItems);

            var configItems = await _entityCache.ConfigurationItems();
            var configItem = configItems.FirstOrDefault(c => c.Id == request.Id);

            response.Notifications.Add($"Configuration item '{configItem.Key}' has been updated", NotificationTypeEnum.Success);
            return response;
        }

        public async Task<CreateConfigurationItemResponse> CreateConfigurationItem(CreateConfigurationItemRequest request, int userId)
        {
            var response = new CreateConfigurationItemResponse();

            int id;
            using (var uow = _uowFactory.GetUnitOfWork())
            {
                id = await uow.ConfigurationRepo.CreateConfigurationItem(new Infrastructure.Repositories.ConfigurationRepo.Models.CreateConfigurationItemRequest()
                {
                    Key = request.Key,
                    Description = request.Description,
                    Boolean_Value = request.BooleanValue,
                    DateTime_Value = request.DateTimeValue,
                    Decimal_Value = request.DecimalValue,
                    Int_Value = request.IntValue,
                    Money_Value = request.MoneyValue,
                    String_Value = request.StringValue,
                    Created_By = userId
                });
                uow.Commit();
            }

            _entityCache.Remove(CacheConstants.ConfigurationItems);

            var configItems = await _entityCache.ConfigurationItems();
            var configItem = configItems.FirstOrDefault(c => c.Id == id);

            response.Notifications.Add($"Configuration item '{configItem.Key}' has been created", NotificationTypeEnum.Success);
            return response;
        }

        #endregion

        #endregion
    }
}
