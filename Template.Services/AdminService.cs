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
using Template.Models.ServiceModels.Admin.ClaimManagement;
using Template.Services.Contracts;

namespace Template.Services
{
    public class AdminService : IAdminService
    {
        #region Instance Fields

        private readonly ILogger<AdminService> _logger;

        private readonly IEmailService _emailService;
        private readonly IAccountService _accountService;
        private readonly ISessionService _sessionService;

        private readonly IUnitOfWorkFactory _uowFactory;
        private readonly IEntityCache _entityCache;

        #endregion

        #region Constructor

        public AdminService(
            ILogger<AdminService> logger,
            IEmailService emailService,
            IAccountService accountService,
            ISessionService sessionService,
            IUnitOfWorkFactory uowFactory,
            IEntityCache entityCache)
        {
            _logger = logger;

            _uowFactory = uowFactory;

            _entityCache = entityCache;
            _emailService = emailService;
            _accountService = accountService;
            _sessionService = sessionService;
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

        public async Task<EnableUserResponse> EnableUser(EnableUserRequest request)
        {
            var session = await _sessionService.GetAuthenticatedSession();
            var response = new EnableUserResponse();

            UserEntity user;
            using (var uow = _uowFactory.GetUnitOfWork())
            {
                user = await uow.UserRepo.GetUserById(new Infrastructure.Repositories.UserRepo.Models.GetUserByIdRequest()
                {
                    User_Id = request.Id
                });

                await uow.UserRepo.EnableUser(new Infrastructure.Repositories.UserRepo.Models.EnableUserRequest()
                {
                    User_Id = request.Id,
                    Updated_By = session.User.Id
                });
                uow.Commit();
            }

            response.Notifications.Add($"User '{user.Username}' has been enabled", NotificationTypeEnum.Success);
            return response;
        }

        public async Task<DisableUserResponse> DisableUser(DisableUserRequest request)
        {
            var session = await _sessionService.GetAuthenticatedSession();
            var response = new DisableUserResponse();

            UserEntity user;
            using (var uow = _uowFactory.GetUnitOfWork())
            {
                user = await uow.UserRepo.GetUserById(new Infrastructure.Repositories.UserRepo.Models.GetUserByIdRequest()
                {
                    User_Id = request.Id
                });

                await uow.UserRepo.DisableUser(new Infrastructure.Repositories.UserRepo.Models.DisableUserRequest()
                {
                    Id = request.Id
                });
                uow.Commit();
            }

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

        public async Task<CreateUserResponse> CreateUser(CreateUserRequest request)
        {
            var session = await _sessionService.GetAuthenticatedSession();
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

            var password = "";

            int id;
            using (var uow = _uowFactory.GetUnitOfWork())
            {
                id = await uow.UserRepo.CreateUser(new Infrastructure.Repositories.UserRepo.Models.CreateUserRequest()
                {
                    Username = username,
                    First_Name = request.FirstName,
                    Last_Name = request.LastName,
                    Mobile_Number = request.MobileNumber,
                    Email_Address = request.EmailAddress,
                    Password_Hash = password,
                    Created_By = session.User.Id,
                    Is_Enabled = true
                });
                uow.Commit();
            }

            await CreateOrDeleteUserRoles(request.RoleIds, id, session.User.Id);

            response.Notifications.Add($"User {request.Username} has been created", NotificationTypeEnum.Success);
            return response;
        }

        public async Task<UpdateUserResponse> UpdateUser(UpdateUserRequest request)
        {
            var session = await _sessionService.GetAuthenticatedSession();
            var response = new UpdateUserResponse();

            // todo: duplicate check

            using (var uow = _uowFactory.GetUnitOfWork())
            {
                var user = await uow.UserRepo.GetUserById(new Infrastructure.Repositories.UserRepo.Models.GetUserByIdRequest()
                {
                    User_Id = request.UserId
                });

                var dbRequest = new Infrastructure.Repositories.UserRepo.Models.UpdateUserRequest()
                {
                    Id = request.UserId,
                    Username = request.Username,
                    First_Name = request.FirstName,
                    Last_Name = request.LastName,
                    Mobile_Number = request.MobileNumber,
                    Email_Address = request.EmailAddress,
                    Password_Hash = user.Password_Hash,
                    Is_Locked_Out = request.Is_Locked_Out,
                    Lockout_End = request.Lockout_End,
                    Registration_Confirmed = request.RegistrationConfirmed,
                    Updated_By = session.User.Id,
                    Is_Enabled = true
                };

                if (!string.IsNullOrEmpty(request.Password))
                {
                    dbRequest.Password_Hash = BCrypt.Net.BCrypt.HashPassword(request.Password);
                }

                await uow.UserRepo.UpdateUser(dbRequest);
                uow.Commit();
            }

            await CreateOrDeleteUserRoles(request.RoleIds, request.UserId, session.User.Id);

            response.Notifications.Add($"User {request.Username} has been updated", NotificationTypeEnum.Success);
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

        public async Task<EnableRoleResponse> EnableRole(EnableRoleRequest request)
        {
            var session = await _sessionService.GetAuthenticatedSession();
            var response = new EnableRoleResponse();

            var roles = await _entityCache.Roles();
            var role = roles.FirstOrDefault(u => u.Id == request.RoleId);

            using (var uow = _uowFactory.GetUnitOfWork())
            {
                await uow.UserRepo.EnableRole(new Infrastructure.Repositories.UserRepo.Models.EnableRoleRequest()
                {
                    Role_Id = role.Id,
                    Updated_By = session.User.Id
                });
                uow.Commit();
            }

            _entityCache.Remove(CacheConstants.Roles);

            response.Notifications.Add($"Role '{role.Name}' has been enabled", NotificationTypeEnum.Success);
            return response;
        }

        public async Task<DisableRoleResponse> DisableRole(DisableRoleRequest request)
        {
            var session = await _sessionService.GetAuthenticatedSession();
            var response = new DisableRoleResponse();

            var roles = await _entityCache.Roles();
            var role = roles.FirstOrDefault(u => u.Id == request.Id);

            using (var uow = _uowFactory.GetUnitOfWork())
            {
                await uow.UserRepo.DisableRole(new Infrastructure.Repositories.UserRepo.Models.DisableRoleRequest()
                {
                    Id = role.Id,
                    Updated_By = session.User.Id
                });
                uow.Commit();
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

        public async Task<CreateRoleResponse> CreateRole(CreateRoleRequest request)
        {
            var session = await _sessionService.GetAuthenticatedSession();
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

            int id;
            using (var uow = _uowFactory.GetUnitOfWork())
            {
                id = await uow.UserRepo.CreateRole(new Infrastructure.Repositories.UserRepo.Models.CreateRoleRequest()
                {
                    Name = request.Name,
                    Description = request.Description,
                    Created_By = session.User.Id,
                });
                uow.Commit();
            }

            _entityCache.Remove(CacheConstants.Roles);

            var roles = await _entityCache.Roles();
            var role = roles.FirstOrDefault(r => r.Id == id);

            await CreateOrDeleteRoleClaims(request.ClaimIds, id, session.User.Id);

            response.Notifications.Add($"Role '{request.Name}' has been created", NotificationTypeEnum.Success);
            return response;
        }

        public async Task<UpdateRoleResponse> UpdateRole(UpdateRoleRequest request)
        {
            var session = await _sessionService.GetAuthenticatedSession();
            var response = new UpdateRoleResponse();

            var roles = await _entityCache.Roles();
            var role = roles.FirstOrDefault(u => u.Id == request.RoleId);

            using (var uow = _uowFactory.GetUnitOfWork())
            {
                await uow.UserRepo.UpdateRole(new Infrastructure.Repositories.UserRepo.Models.UpdateRoleRequest()
                {
                    Name = request.Name,
                    Description = request.Description,
                    Updated_By = session.User.Id
                });
                uow.Commit();
            }

            await CreateOrDeleteRoleClaims(request.ClaimIds, request.RoleId, session.User.Id);

            _entityCache.Remove(CacheConstants.Roles);
            _entityCache.Remove(CacheConstants.RoleClaims);

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

        public async Task<UpdateConfigurationItemResponse> UpdateConfigurationItem(UpdateConfigurationItemRequest request)
        {
            var session = await _sessionService.GetAuthenticatedSession();
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
                    Updated_By = session.User.Id
                });
                uow.Commit();
            }

            _entityCache.Remove(CacheConstants.ConfigurationItems);

            var configItems = await _entityCache.ConfigurationItems();
            var configItem = configItems.FirstOrDefault(c => c.Id == request.Id);

            response.Notifications.Add($"Configuration item '{configItem.Key}' has been updated", NotificationTypeEnum.Success);
            return response;
        }

        public async Task<CreateConfigurationItemResponse> CreateConfigurationItem(CreateConfigurationItemRequest request)
        {
            var session = await _sessionService.GetAuthenticatedSession();
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
                    Created_By = session.User.Id
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

        #region Sessions

        public async Task<GetSessionResponse> GetSession(GetSessionRequest request)
        {
            var response = new GetSessionResponse();

            using (var uow = _uowFactory.GetUnitOfWork())
            {
                var session = await uow.SessionRepo.GetSessionById(new Infrastructure.Repositories.SessionRepo.Models.GetSessionByIdRequest()
                {
                    Id = request.Id
                });

                if (session == null)
                {
                    response.Notifications.AddError($"Could not find session with Id {request.Id}");
                    return response;
                }

                var logs = await uow.SessionRepo.GetSessionLogsBySessionId(new Infrastructure.Repositories.SessionRepo.Models.GetSessionLogsBySessionIdRequest()
                {
                    Session_Id = request.Id
                });
                var events = await uow.SessionRepo.GetSessionLogEventsBySessionId(new Infrastructure.Repositories.SessionRepo.Models.GetSessionLogEventsBySessionIdRequest()
                {
                    Session_Id = request.Id
                });

                if (session.User_Id.HasValue)
                {
                    var user = await uow.UserRepo.GetUserById(new Infrastructure.Repositories.UserRepo.Models.GetUserByIdRequest()
                    {
                        User_Id = session.User_Id.Value
                    });
                    response.User = user;
                }

                var eventsLookup = await _entityCache.SessionEvents();
                var eventIds = events.Select(e => e.Id);

                response.Session = session;
                response.Logs = logs.Select(l => new SessionLog()
                {
                    Action = l.Action,
                    Controller = l.Controller,
                    Events = eventsLookup.Where(el => eventIds.Contains(el.Id)).ToList()
                }).ToList();

                uow.Commit();
                return response;
            }
        }

        public async Task<GetSessionsResponse> GetSessions(GetSessionsRequest request)
        {
            var response = new GetSessionsResponse();

            if (request.Last24Hours.HasValue && request.Last24Hours.Value)
            {
                using (var uow = _uowFactory.GetUnitOfWork())
                {
                    response.Sessions = await uow.SessionRepo.GetSessionsByStartDate(new Infrastructure.Repositories.SessionRepo.Models.GetSessionsByStartDateRequest()
                    {
                        Start_Date = DateTime.Now.AddDays(-1)
                    });

                    uow.Commit();
                }
                response.SelectedFilter = "Last 24 Hours";
            }

            if (request.LastXDays.HasValue)
            {
                using (var uow = _uowFactory.GetUnitOfWork())
                {
                    response.Sessions = await uow.SessionRepo.GetSessionsByStartDate(new Infrastructure.Repositories.SessionRepo.Models.GetSessionsByStartDateRequest()
                    {
                        Start_Date = DateTime.Today.AddDays(request.LastXDays.Value * -1)
                    });

                    uow.Commit();
                }
                response.SelectedFilter = $"Last {request.LastXDays} days";
            }

            if (request.Day.HasValue)
            {
                using (var uow = _uowFactory.GetUnitOfWork())
                {
                    response.Sessions = await uow.SessionRepo.GetSessionsByDate(new Infrastructure.Repositories.SessionRepo.Models.GetSessionsByDateRequest()
                    {
                        Date = request.Day.Value
                    });

                    uow.Commit();
                }
                response.SelectedFilter = request.Day.Value.ToLongDateString();
            }

            if (request.UserId.HasValue)
            {
                using (var uow = _uowFactory.GetUnitOfWork())
                {
                    response.Sessions = await uow.SessionRepo.GetSessionsByUserId(new Infrastructure.Repositories.SessionRepo.Models.GetSessionsByUserIdRequest()
                    {
                        User_Id = request.UserId.Value
                    });

                    uow.Commit();
                }
                response.SelectedFilter = $"User ID: {request.UserId}";
            }

            if (!string.IsNullOrEmpty(request.Username))
            {
                using (var uow = _uowFactory.GetUnitOfWork())
                {
                    var user = await uow.UserRepo.GetUserByUsername(new Infrastructure.Repositories.UserRepo.Models.GetUserByUsernameRequest()
                    {
                        Username = request.Username
                    });

                    if (user == null)
                    {
                        response.Notifications.AddError($"Could not find user with username {request.Username}");
                        return response;
                    }

                    response.Sessions = await uow.SessionRepo.GetSessionsByUserId(new Infrastructure.Repositories.SessionRepo.Models.GetSessionsByUserIdRequest()
                    {
                        User_Id = user.Id
                    });

                    uow.Commit();
                }
                response.SelectedFilter = $"Username: {request.Username}";
            }

            if (!string.IsNullOrEmpty(request.MobileNumber))
            {
                using (var uow = _uowFactory.GetUnitOfWork())
                {
                    var user = await uow.UserRepo.GetUserByMobileNumber(new Infrastructure.Repositories.UserRepo.Models.GetUserByMobileNumberRequest()
                    {
                        Mobile_Number = request.MobileNumber
                    });

                    if (user == null)
                    {
                        response.Notifications.AddError($"Could not find user with mobile number {request.MobileNumber}");
                        return response;
                    }

                    response.Sessions = await uow.SessionRepo.GetSessionsByUserId(new Infrastructure.Repositories.SessionRepo.Models.GetSessionsByUserIdRequest()
                    {
                        User_Id = user.Id
                    });

                    uow.Commit();
                }
                response.SelectedFilter = $"Mobile Number: {request.MobileNumber}";
            }

            if (!string.IsNullOrEmpty(request.EmailAddress))
            {
                using (var uow = _uowFactory.GetUnitOfWork())
                {
                    var user = await uow.UserRepo.GetUserByEmail(new Infrastructure.Repositories.UserRepo.Models.GetUserByEmailRequest()
                    {
                        Email_Address = request.EmailAddress
                    });

                    if (user == null)
                    {
                        response.Notifications.AddError($"Could not find user with email address {request.EmailAddress}");
                        return response;
                    }

                    response.Sessions = await uow.SessionRepo.GetSessionsByUserId(new Infrastructure.Repositories.SessionRepo.Models.GetSessionsByUserIdRequest()
                    {
                        User_Id = user.Id
                    });

                    uow.Commit();
                }
                response.SelectedFilter = $"Email Address: {request.EmailAddress}";
            }

            response.Sessions = response.Sessions.OrderByDescending(s => s.Created_Date).ToList();
            return response;
        }

        public async Task<GetSessionEventManagementResponse> GetSessionEventManagement()
        {
            var response = new GetSessionEventManagementResponse();

            response.SessionEvents = await _entityCache.SessionEvents();

            return response;
        }

        public async Task<GetSessionEventResponse> GetSessionEvent(GetSessionEventRequest request)
        {
            var response = new GetSessionEventResponse();

            var sessionEvents = await _entityCache.SessionEvents();
            var sessionEvent = sessionEvents.FirstOrDefault(c => c.Id == request.Id);

            response.SessionEvent = sessionEvent;

            return response;
        }

        public async Task<UpdateSessionEventResponse> UpdateSessionEvent(UpdateSessionEventRequest request)
        {
            var session = await _sessionService.GetAuthenticatedSession();
            var response = new UpdateSessionEventResponse();

            using (var uow = _uowFactory.GetUnitOfWork())
            {
                await uow.SessionRepo.UpdateSessionEvent(new Infrastructure.Repositories.SessionRepo.Models.UpdateSessionEventRequest()
                {
                    Id = request.Id,
                    Description = request.Description,
                    Updated_By = session.User.Id
                });
                uow.Commit();
            }

            _entityCache.Remove(CacheConstants.SessionEvents);

            var sessionEvents = await _entityCache.SessionEvents();
            var sessionEvent = sessionEvents.FirstOrDefault(c => c.Id == request.Id);

            response.Notifications.Add($"Session event '{sessionEvent.Key}' has been updated", NotificationTypeEnum.Success);
            return response;
        }

        public async Task<CreateSessionEventResponse> CreateSessionEvent(CreateSessionEventRequest request)
        {
            var session = await _sessionService.GetAuthenticatedSession();
            var response = new CreateSessionEventResponse();

            int id;
            using (var uow = _uowFactory.GetUnitOfWork())
            {
                id = await uow.SessionRepo.CreateSessionEvent(new Infrastructure.Repositories.SessionRepo.Models.CreateSessionEventRequest()
                {
                    Key = request.Key,
                    Description = request.Description,
                    Created_By = session.User.Id
                });
                uow.Commit();
            }

            _entityCache.Remove(CacheConstants.SessionEvents);

            var sessionEvents = await _entityCache.SessionEvents();
            var sessionEvent = sessionEvents.FirstOrDefault(c => c.Id == id);

            response.Notifications.Add($"Session event '{sessionEvent.Key}' has been created", NotificationTypeEnum.Success);
            return response;
        }

        #endregion

        #region Claim Management 

        public Task<GetClaimManagementResponse> GetClaimManagement()
        {
            throw new NotImplementedException();
        }

        public Task<GetClaimResponse> GetClaim(GetClaimRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<UpdateClaimResponse> UpdateClaim(UpdateClaimRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<CreateClaimResponse> CreateClaim(CreateClaimRequest request)
        {
            throw new NotImplementedException();
        }

        #endregion

        #endregion
    }
}
