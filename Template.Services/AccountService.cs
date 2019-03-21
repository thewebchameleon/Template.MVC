﻿using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Template.Common.Extensions;
using Template.Common.Notifications;
using Template.Infrastructure.Authentication;
using Template.Infrastructure.Cache;
using Template.Infrastructure.Cache.Contracts;
using Template.Infrastructure.Configuration;
using Template.Infrastructure.Session;
using Template.Infrastructure.Session.Contracts;
using Template.Infrastructure.UnitOfWork.Contracts;
using Template.Models.DomainModels;
using Template.Models.ServiceModels;
using Template.Models.ServiceModels.Account;
using Template.Services.Contracts;

namespace Template.Services
{
    public class AccountService : IAccountService
    {
        #region Instance Fields

        private readonly ILogger<AccountService> _logger;

        private readonly IEmailService _emailService;
        private readonly ISessionService _sessionService;

        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUnitOfWorkFactory _uowFactory;
        private readonly IEntityCache _entityCache;
        private readonly ISessionProvider _sessionProvider;

        #endregion

        #region Constructor

        public AccountService(
            ILogger<AccountService> logger,
            IEmailService emailService,
            ISessionService sessionService,
            IUnitOfWorkFactory uowFactory,
            IEntityCache entityCache,
            ISessionProvider sessionProvider,
            IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;

            _httpContextAccessor = httpContextAccessor;
            _uowFactory = uowFactory;
            _sessionProvider = sessionProvider;

            _entityCache = entityCache;
            _emailService = emailService;
            _sessionService = sessionService;
        }

        #endregion

        #region Public Methods

        public async Task<RegisterResponse> Register(RegisterRequest request)
        {
            var response = new RegisterResponse();
            var username = request.EmailAddress;

            var duplicateResponse = await DuplicateUserCheck(new DuplicateUserCheckRequest()
            {
                EmailAddress = request.EmailAddress,
                Username = username
            });

            if (duplicateResponse.Notifications.HasErrors)
            {
                response.Notifications.Add(duplicateResponse.Notifications);
                return response;
            }

            int id;
            using (var uow = _uowFactory.GetUnitOfWork())
            {
                id = await uow.UserRepo.CreateUser(new Infrastructure.Repositories.UserRepo.Models.CreateUserRequest()
                {
                    Username = username,
                    Email_Address = request.EmailAddress,
                    Password_Hash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                    Is_Enabled = true,
                    Created_By = ApplicationConstants.SystemUserId,
                });
                uow.Commit();
            }

            await _emailService.SendAccountActivationEmail(new SendAccountActivationEmailRequest()
            {
                UserID = id
            });

            _entityCache.Remove(CacheConstants.UserRoles);

            response.Notifications.Add($"You have been successfully registered, please check {request.EmailAddress} for an activation link", NotificationTypeEnum.Success);
            return response;
        }

        public async Task<LoginResponse> Login(LoginRequest request)
        {
            var session = await _sessionService.GetSession();
            var response = new LoginResponse();

            UserEntity user;
            using (var uow = _uowFactory.GetUnitOfWork())
            {
                user = await uow.UserRepo.GetUserByUsername(new Infrastructure.Repositories.UserRepo.Models.GetUserByUsernameRequest()
                {
                    Username = request.Username
                });

                if (!BCrypt.Net.BCrypt.Verify(request.Password, user.Password_Hash))
                {
                    response.Notifications.AddError("Username or password is incorrect");
                    return response;
                }

                if (!user.Is_Enabled)
                {
                    response.Notifications.AddError("Your account has been disabled, please contact the website administrator");
                    return response;
                }

                if (user.Is_Locked_Out)
                {
                    response.Notifications.AddError("Your account has been locked due to too many invalid login attempts, please try again later");
                    return response;
                }

                var sessionEntity = await uow.SessionRepo.AddUserToSession(new Infrastructure.Repositories.SessionRepo.Models.AddUserToSessionRequest()
                {
                    Id = session.Id,
                    User_Id = user.Id,
                    Updated_By = ApplicationConstants.SystemUserId
                });
                uow.Commit();

                await _sessionProvider.Set(SessionConstants.SessionEntity, sessionEntity);
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimConstants.SessionId, session.Id.ToString()),
            };

            var claimsIdentity = new ClaimsIdentity(
                claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var properties = ApplicationConstants.AuthenticationProperties;
            properties.IsPersistent = request.RememberMe;

            await _httpContextAccessor.HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                properties);

            return response;
        }

        public void Logout()
        {
            _httpContextAccessor.HttpContext.SignOutAsync().Wait();
        }

        public Task<ForgotPasswordResponse> ForgotPassword(ForgotPasswordRequest request)
        {
            throw new NotImplementedException();
        }

        public async Task<GetProfileResponse> GetProfile()
        {
            var session = await _sessionService.GetAuthenticatedSession();

            var user = session.User;
            var roles = await _entityCache.Roles();
            var userRoles = await _entityCache.UserRoles();
            var usersRoles = userRoles.Where(ur => ur.User_Id == session.User.Id).Select(ur => ur.Role_Id);

            return new GetProfileResponse()
            {
                EmailAddress = user.Email_Address,
                FirstName = user.First_Name,
                LastName = user.Last_Name,
                MobileNumber = user.Mobile_Number,
                Username = user.Username,
                Roles = roles.Where(r => usersRoles.Contains(r.Id)).ToList()
            };
        }

        public async Task<UpdateProfileResponse> UpdateProfile(UpdateProfileRequest request)
        {
            var response = new UpdateProfileResponse();
            var session = await _sessionService.GetAuthenticatedSession();

            var dbRequest = new Infrastructure.Repositories.UserRepo.Models.UpdateUserRequest()
            {
                Id = session.User.Id,
                Username = request.Username,
                Email_Address = request.EmailAddress,
                First_Name = request.FirstName,
                Last_Name = request.LastName,
                Mobile_Number = request.MobileNumber,
                Password_Hash = session.User.Password_Hash,
                Is_Enabled = session.User.Is_Enabled,
                Is_Locked_Out = session.User.Is_Locked_Out,
                Lockout_End = session.User.Lockout_End,
                Registration_Confirmed = session.User.Registration_Confirmed,
                Updated_By = session.User.Id
            };

            if (!string.IsNullOrEmpty(request.Password))
            {
                dbRequest.Password_Hash = BCrypt.Net.BCrypt.HashPassword(request.Password);
            }

            using (var uow = _uowFactory.GetUnitOfWork())
            {
                await uow.UserRepo.UpdateUser(dbRequest);
                uow.Commit();
            }

            // rehydrate user in session
            await _sessionProvider.Remove(SessionConstants.UserEntity);

            response.Notifications.Add("Profile updated successfully", NotificationTypeEnum.Success);
            return response;
        }

        public async Task<DuplicateUserCheckResponse> DuplicateUserCheck(DuplicateUserCheckRequest request)
        {
            var response = new DuplicateUserCheckResponse();
            var emailAddress = request.EmailAddress.SafeTrim();
            var mobileNumber = request.MobileNumber.SafeTrim();
            var username = request.EmailAddress.SafeTrim();

            var duplicateUserRequest = new Infrastructure.Repositories.UserRepo.Models.FetchDuplicateUserRequest()
            {
                Username = username,
                Email_Address = emailAddress,
                Mobile_Number = mobileNumber,
            };

            // this is for when the user wants to change their email / username or mobile number
            if (request.UserId != null)
            {
                duplicateUserRequest.User_Id = request.UserId.Value;
            }

            using (var uow = _uowFactory.GetUnitOfWork())
            {
                var user = await uow.UserRepo.FetchDuplicateUser(duplicateUserRequest);
                uow.Commit();

                if (user == null)
                {
                    // no duplicate found
                    return new DuplicateUserCheckResponse();
                }

                bool matchFound = false;
                if (string.Equals(user.Email_Address, emailAddress, StringComparison.InvariantCultureIgnoreCase))
                {
                    response.Notifications.AddError("A user is already registered with this email address");
                    matchFound = true;
                }

                if (string.Equals(user.Mobile_Number, mobileNumber, StringComparison.InvariantCultureIgnoreCase))
                {
                    response.Notifications.AddError("A user is already registered with this mobile number");
                    matchFound = true;
                }

                if (string.Equals(user.Username, username, StringComparison.InvariantCultureIgnoreCase))
                {
                    response.Notifications.AddError("A user is already registered with this username");
                    matchFound = true;
                }

                if (user.Is_Deleted || !user.Is_Enabled)
                {
                    response.Notifications.AddError("This user has been removed / disabled");
                    matchFound = true;
                }

                if (matchFound)
                {
                    return response;
                }
                _logger.LogError("Duplicate user found but could not determine why", $"UserId: {user.Id}", duplicateUserRequest);
                response.Notifications.AddError("An error ocurred while performing a duplicate check");
                return response;
            }
        }

        public async Task<DuplicateRoleCheckResponse> DuplicateRoleCheck(DuplicateRoleCheckRequest request)
        {
            var response = new DuplicateRoleCheckResponse();

            var roles = await _entityCache.Roles();
            var matchFound = roles.Any(u => string.Equals(u.Name, request.Name, StringComparison.InvariantCultureIgnoreCase));

            if (matchFound)
            {
                response.Notifications.AddError($"There is already a role with the name {request.Name}");
                return response;
            }
            return response;
        }

        //public async Task<ActivateAccountResponse> ActivateAccount(ActivateAccountRequest request, int userId)
        //{
        //    var tokens = await _entityCache.Tokens();
        //    var token = tokens.FirstOrDefault(t => t.Value == request.Token);

        //    if (token == null)
        //    {
        //        _logger.LogWarning($"Token does not exist: {request.Token}");
        //        throw new BusinessException("An invalid token was provided");
        //    }

        //    if (token.ExpiryDate < DateTime.Now)
        //    {
        //        throw new BusinessException("Token has expired");
        //    }

        //    using (var uow = _uowFactory.GetUnitOfWork())
        //    {
        //        await uow.UserRepo.ActivateAccount(new Infrastructure.Repositories.UserRepo.Models.ActivateAccountRequest()
        //        {
        //            Token = request.Token
        //        });

        //        uow.Commit();
        //    }

        //    _entityCache.Clear(CacheConstants.Tokens);

        //    return new ActivateAccountResponse();
        //}

        //public async Task<GetUserByIdResponse> GetUserById(GetUserByIdRequest request, int userId)
        //{
        //    var user = await _userManager.FindByIdAsync(request.UserId.ToString());
        //    var roles = await _userManager.GetRolesAsync(user);

        //    return new GetUserByIdResponse()
        //    {
        //        User = new User()
        //        {
        //            Id = user.Id,
        //            Configuration = user.Configuration,
        //            Email = user.Email,
        //            FirstName = user.FirstName,
        //            IsEnabled = user.IsEnabled,
        //            IsLockedOut = user.IsLockedOut,
        //            Username = user.Username,
        //            MobileNumber = user.MobileNumber,
        //            LastName = user.LastName,
        //            Suburb = user.Suburb,
        //            Roles = roles.ToList()
        //        }
        //    };
        //}

        //public async Task<CreateUserResponse> CreateUser(CreateUserRequest request, int userId)
        //{
        //    var user = new Infrastructure.Repositories.UserRepo.Models.User()
        //    {
        //        Username = request.MobileNumber, //TODO: what is the default username?
        //        FirstName = request.FirstName,
        //        Email = request.Email,
        //        MobileNumber = request.MobileNumber
        //    };

        //    await _userManager.AddPasswordAsync(user, request.Password);

        //    var result = await _userManager.CreateAsync(user);
        //    if (!result.Succeeded)
        //    {
        //        throw new Exception(string.Join(",", result.Errors.Select(e => e.Description)));
        //    }

        //    try
        //    {
        //        result = await _userManager.AddToRolesAsync(user, request.Roles.Distinct());
        //    }
        //    catch
        //    {
        //        await DeleteUser(new DeleteUserRequest()
        //        {
        //            UserId = user.Id
        //        }, userId);
        //        throw;
        //    }

        //    if (!result.Succeeded)
        //    {
        //        await DeleteUser(new DeleteUserRequest()
        //        {
        //            UserId = user.Id
        //        }, userId);
        //        throw new Exception(string.Join(",", result.Errors.Select(e => e.Description)));
        //    }

        //    var users = await _entityCache.Users();
        //    user = users.FirstOrDefault(u => u.Id == user.Id);

        //    var roles = await _userManager.GetRolesAsync(user);

        //    return new CreateUserResponse()
        //    {
        //        User = new User()
        //        {
        //            Id = user.Id,
        //            Configuration = user.Configuration,
        //            Email = user.Configuration,
        //            FirstName = user.FirstName,
        //            IsEnabled = user.IsEnabled,
        //            IsLockedOut = user.IsLockedOut,
        //            MobileNumber = user.MobileNumber,
        //            Username = user.Username,
        //            Roles = roles.ToList()
        //        }
        //    };
        //}

        //public async Task<UpdateUserResponse> UpdateUser(UpdateUserRequest request, int userId)
        //{
        //    var user = await _userManager.FindByIdAsync(request.Id.ToString());
        //    var isCurrentPasswordValid = await _userManager.CheckPasswordAsync(user, request.CurrentPassword);

        //    if (!isCurrentPasswordValid)
        //    {
        //        throw new BusinessException("Your password is incorrect, please try again");
        //    }

        //    user.Username = request.Username;
        //    user.FirstName = request.FirstName;
        //    user.LastName = request.LastName;
        //    user.Email = request.Email;
        //    user.Configuration = request.Configuration;
        //    user.MobileNumber = request.MobileNumber;
        //    user.Suburb = request.Suburb;
        //    user.IsEnabled = request.IsEnabled;
        //    user.IsLockedOut = request.IsLockedOut;
        //    user.UpdatedBy = request.Id;

        //    if (!string.IsNullOrEmpty(request.NewPassword))
        //    {
        //        var updatePasswordResult = await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);
        //        if (!updatePasswordResult.Succeeded)
        //        {
        //            throw new Exception(string.Join(",", updatePasswordResult.Errors.Select(e => e.Description)));
        //        }
        //    }

        //    var result = await _userManager.UpdateAsync(user);
        //    if (!result.Succeeded)
        //    {
        //        throw new Exception(string.Join(",", result.Errors.Select(e => e.Description)));
        //    }

        //    if (request.Roles.Any())
        //    {
        //        var userRoles = await _userManager.GetRolesAsync(user);

        //        var rolesToRemove = userRoles.Except(request.Roles);
        //        var rolesToAdd = request.Roles.Except(userRoles).Distinct();

        //        if (rolesToRemove.Any())
        //        {
        //            foreach (var role in rolesToRemove)
        //            {
        //                await _userManager.RemoveFromRoleAsync(user, role);
        //            }

        //            if (!result.Succeeded)
        //            {
        //                throw new Exception(string.Join(",", result.Errors.Select(e => e.Description)));
        //            }
        //        }

        //        if (rolesToAdd.Any())
        //        {
        //            foreach (var role in rolesToAdd)
        //            {
        //                await _userManager.AddToRoleAsync(user, role);
        //            }

        //            if (!result.Succeeded)
        //            {
        //                throw new Exception(string.Join(",", result.Errors.Select(e => e.Description)));
        //            }
        //        }

        //        if (rolesToRemove.Any() || rolesToAdd.Any())
        //        {
        //            _entityCache.Remove(CacheConstants.UserRoles);
        //        }
        //    }
        //    _entityCache.Remove(CacheConstants.UserRoles);
        //    _entityCache.Remove(CacheConstants.Users);

        //    return new UpdateUserResponse()
        //    {
        //        User = new User()
        //        {
        //            Id = user.Id,
        //            Configuration = user.Configuration,
        //            Email = user.Email,
        //            FirstName = user.FirstName,
        //            LastName = user.LastName,
        //            Suburb = user.Suburb,
        //            MobileNumber = user.MobileNumber,
        //            IsEnabled = user.IsEnabled,
        //            IsLockedOut = user.IsLockedOut,
        //            Username = user.Username,
        //            Roles = request.Roles.Distinct().ToList()
        //        }
        //    };
        //}

        //public async Task<DeleteUserResponse> DeleteUser(DeleteUserRequest request, int userId)
        //{
        //    var user = await _userManager.FindByIdAsync(request.UserId.ToString());

        //    var result = await _userManager.DeleteAsync(user);
        //    if (!result.Succeeded)
        //    {
        //        throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));
        //    }

        //    _entityCache.Remove(CacheConstants.Users);

        //    return new DeleteUserResponse();
        //}

        //public async Task<GetRoleByIdResponse> GetRoleById(GetRoleByIdRequest request, int userId)
        //{
        //    var role = await _roleManager.FindByIdAsync(request.RoleId.ToString());
        //    var claims = await _roleManager.GetClaimsAsync(role);

        //    return new GetRoleByIdResponse()
        //    {
        //        Role = new Role()
        //        {
        //            Id = role.Id,
        //            Description = role.Description,
        //            Name = role.Name,
        //            Permissions = claims.Select(c => ApplicationPermissions.GetPermissionByValue(c.Value)).ToList()
        //        }
        //    };
        //}

        //public async Task<CreateRoleResponse> CreateRole(CreateRoleRequest request, int userId)
        //{
        //    var role = new Infrastructure.Repositories.UserRepo.Models.Role()
        //    {
        //        Name = request.Name,
        //        NameNormalized = request.Name.ToUpper(),
        //        Description = request.Description,
        //        CreatedBy = request.CreatedBy
        //    };

        //    var result = await _roleManager.CreateAsync(role);
        //    if (!result.Succeeded)
        //    {
        //        throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));
        //    }

        //    foreach (var claim in request.Permissions.Distinct())
        //    {
        //        await _roleManager.AddClaimAsync(role, new Claim(ClaimConstants.Permission, ApplicationPermissions.GetPermissionByValue(claim.Value)));
        //    }
        //    _entityCache.Remove(CacheConstants.RoleClaims);
        //    _entityCache.Remove(CacheConstants.Roles);

        //    return new CreateRoleResponse()
        //    {
        //        Role = new Role()
        //        {
        //            Id = role.Id,
        //            Name = role.Name,
        //            Description = role.Description,
        //            Permissions = request.Permissions
        //        }
        //    };
        //}

        //public async Task<UpdateRoleResponse> UpdateRole(UpdateRoleRequest request, int userId)
        //{
        //    var roles = await _entityCache.Roles();
        //    var role = roles.FirstOrDefault(r => r.Id == request.Id);

        //    if (role == null)
        //    {
        //        throw new Exception("Could not find role");
        //    }

        //    role.Name = request.Name;
        //    role.Description = request.Description;
        //    role.UpdatedBy = userId;

        //    await _roleManager.UpdateAsync(role);

        //    var existingClaims = await _roleManager.GetClaimsAsync(role);
        //    var existingClaimsValues = existingClaims.Select(c => c.Value);

        //    request.Permissions = request.Permissions.Distinct().ToList();

        //    var claimsToAdd = request.Permissions.Select(p => p.Value).Where(c => !existingClaimsValues.Contains(c));
        //    if (claimsToAdd.Any())
        //    {
        //        foreach (var claim in claimsToAdd)
        //        {
        //            await _roleManager.AddClaimAsync(role, new Claim(ClaimConstants.Permission, ApplicationPermissions.GetPermissionByValue(claim)));
        //        }
        //    }

        //    var claimsToRemove = existingClaimsValues.Where(c => !request.Permissions.Select(p => p.Value).Contains(c));
        //    if (claimsToRemove.Any())
        //    {
        //        foreach (var claim in claimsToRemove)
        //        {
        //            await _roleManager.RemoveClaimAsync(role, new Claim(ClaimConstants.Permission, ApplicationPermissions.GetPermissionByValue(claim)));
        //        }
        //    }

        //    _entityCache.Remove(CacheConstants.RoleClaims);
        //    _entityCache.Remove(CacheConstants.Roles);

        //    return new UpdateRoleResponse()
        //    {
        //        Role = new Role()
        //        {
        //            Id = role.Id,
        //            Name = role.Name,
        //            Description = role.Description,
        //            Permissions = request.Permissions
        //        }
        //    };
        //}

        //public async Task<DeleteRoleResponse> DeleteRole(DeleteRoleRequest request, int userId)
        //{
        //    var role = await _roleManager.FindByIdAsync(request.Id.ToString());

        //    if (role != null)
        //    {
        //        throw new Exception("Could not find role");

        //    }

        //    var result = await _roleManager.DeleteAsync(role);
        //    if (!result.Succeeded)
        //    {
        //        throw new Exception(string.Join(",", result.Errors.Select(e => e.Description)));
        //    }
        //    _entityCache.Remove(CacheConstants.Roles);

        //    return new DeleteRoleResponse();
        //}

        //public async Task<GetUsersResponse> GetUsers(GetUsersRequest request, int userId)
        //{
        //    var userRoles = await _entityCache.UserRoles();
        //    var roles = await _entityCache.Roles();
        //    var users = await _entityCache.Users();
        //    users = users.OrderByDescending(r => r.Id).ToList();

        //    if (request.Page != -1)
        //    {
        //        users = users.Skip((request.Page - 1) * request.PageSize).ToList();
        //    }

        //    if (request.PageSize != -1)
        //    {
        //        users = users.Take(request.PageSize).ToList();
        //    }

        //    var result = new List<User>();
        //    foreach (var user in users)
        //    {
        //        var roleIds = userRoles.Where(ur => ur.UserId == user.Id).Select(ur => ur.RoleId).ToList();
        //        result.Add(new User()
        //        {
        //            Id = user.Id,
        //            Email = user.Email,
        //            Is_Locked_Out = user.Is_Locked_Out,
        //            Username = user.Username,
        //            Mobile_Number = user.Mobile_Number,
        //            Roles = roles.Where(r => roleIds.Contains(r.Id)).Select(r => r.Name).ToList()
        //        });
        //    }

        //    return new GetUsersResponse()
        //    {
        //        Users = result
        //    };
        //}

        //public async Task<UnblockUserResponse> UnblockUser(UnblockUserRequest request, int userId)
        //{
        //    var users = await _entityCache.Users();
        //    var user = users.First(u => u.Id == request.UserId);

        //    if (user == null)
        //    {
        //        throw new Exception("Could not find user");
        //    }

        //    if (!user.IsLockedOut)
        //    {
        //        throw new BusinessException("User is already unblocked");
        //    }

        //    user.IsLockedOut = false;
        //    user.LockoutEnd = null;

        //    await _userManager.UpdateAsync(user);

        //    _entityCache.Remove(CacheConstants.Users);

        //    throw new NotImplementedException();
        //}

        //public async Task<GetRolesResponse> GetRoles(GetRolesRequest request, int userId)
        //{
        //    var userRoles = await _entityCache.UserRoles();
        //    userRoles = userRoles.Where(ur => !ur.Is_Deleted).ToList();

        //    var claims = await _entityCache.RoleClaims();

        //    var roles = await _entityCache.Roles();
        //    roles = roles.Where(r => !r.Is_Deleted).OrderBy(r => r.Name).ToList();

        //    if (request.Page != -1)
        //    {
        //        roles = roles.Skip((request.Page - 1) * request.PageSize).ToList();
        //    }

        //    if (request.PageSize != -1)
        //    {
        //        roles = roles.Take(request.PageSize).ToList();
        //    }

        //    var result = new List<RoleItem>();
        //    foreach (var role in roles)
        //    {
        //        var roleClaims = claims.Where(c => c.RoleId == role.Id && !c.IsDeleted);

        //        result.Add(new RoleItem()
        //        {
        //            Role = new Role()
        //            {
        //                Id = role.Id,
        //                Description = role.Description,
        //                Name = role.Name,
        //                Permissions = roleClaims.Select(c => ApplicationPermissions.GetPermissionByValue(c.ClaimValue)).ToList()
        //            },
        //            UserCount = userRoles.Where(ur => ur.RoleId == role.Id).Count()
        //        });
        //    }
        //    return new GetRolesResponse()
        //    {
        //        Roles = result
        //    };
        //}

        //public async Task<GetRolesByUserIdResponse> GetRolesByUserId(GetRolesByUserIdRequest request, int userId)
        //{
        //    var userRoles = await _entityCache.UserRoles();
        //    var roles = await _entityCache.Roles();

        //    var roleIds = userRoles.Where(ur => ur.UserId == request.UserId).Select(ur => ur.RoleId).ToList();
        //    roles = roles.Where(r => roleIds.Contains(r.Id)).ToList();

        //    return new GetRolesByUserIdResponse()
        //    {
        //        Roles = roles.Select(r => new Role()
        //        {
        //            Id = r.Id,
        //            Description = r.Description,
        //            Name = r.Name
        //        }).ToList()
        //    };
        //}

        //public async Task<UpdateUserPreferencesResponse> UpdateUserPreferences(UpdateUserPreferencesRequest request, int userId)
        //{
        //    var user = await _userManager.FindByIdAsync(request.UserId.ToString());

        //    user.Configuration = request.Configuration;
        //    user.UpdatedBy = request.UserId;

        //    var result = await _userManager.UpdateAsync(user);
        //    if (!result.Succeeded)
        //    {
        //        throw new Exception(string.Join(",", result.Errors.Select(e => e.Description)));
        //    }

        //    _entityCache.Remove(CacheConstants.Users);

        //    return new UpdateUserPreferencesResponse() { Configuration = request.Configuration };
        //}

        #endregion
    }
}
