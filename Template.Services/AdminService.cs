using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Template.Common.Notifications;
using Template.Infrastructure.Authentication;
using Template.Infrastructure.Cache;
using Template.Infrastructure.Cache.Contracts;
using Template.Infrastructure.Session;
using Template.Infrastructure.UnitOfWork.Contracts;
using Template.Models.DomainModels;
using Template.Models.ServiceModels;
using Template.Models.ServiceModels.Admin;
using Template.Models.ServiceModels.Admin.PermissionManagement;
using Template.Models.ServiceModels.Admin.SessionManagement;
using Template.Models.ServiceModels.Admin.UserManagement;
using Template.Services.Contracts;

namespace Template.Services
{
    public class AdminService : IAdminService
    {
        #region Instance Fields

        private readonly ILogger<AdminService> _logger;

        private readonly IAccountService _accountService;
        private readonly ISessionService _sessionService;

        private readonly IUnitOfWorkFactory _uowFactory;
        private readonly IApplicationCache _cache;

        #endregion

        #region Constructor

        public AdminService(
            ILogger<AdminService> logger,
            IAccountService accountService,
            ISessionService sessionService,
            IUnitOfWorkFactory uowFactory,
            IApplicationCache cache)
        {
            _logger = logger;

            _uowFactory = uowFactory;

            _cache = cache;
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
                    Id = request.Id
                });

                await uow.UserRepo.EnableUser(new Infrastructure.Repositories.UserRepo.Models.EnableUserRequest()
                {
                    Id = request.Id,
                    Updated_By = session.User.Entity.Id
                });
                uow.Commit();

                await _sessionService.WriteSessionLogEvent(new Models.ServiceModels.Session.CreateSessionLogEventRequest()
                {
                    EventKey = SessionEventKeys.UserEnabled
                });
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
                    Id = request.Id
                });

                await uow.UserRepo.DisableUser(new Infrastructure.Repositories.UserRepo.Models.DisableUserRequest()
                {
                    Id = request.Id,
                    Updated_By = session.User.Entity.Id
                });
                uow.Commit();

                await _sessionService.WriteSessionLogEvent(new Models.ServiceModels.Session.CreateSessionLogEventRequest()
                {
                    EventKey = SessionEventKeys.UserDisabled
                });
            }

            response.Notifications.Add($"User '{user.Username}' has been disabled", NotificationTypeEnum.Success);
            return response;
        }

        public async Task<GetUserResponse> GetUser(GetUserRequest request)
        {
            var response = new GetUserResponse();

            var userRoles = await _cache.UserRoles();
            var roles = await _cache.Roles();
            var permissions = await _cache.Permissions();

            using (var uow = _uowFactory.GetUnitOfWork())
            {
                var user = await uow.UserRepo.GetUserById(new Infrastructure.Repositories.UserRepo.Models.GetUserByIdRequest()
                {
                    Id = request.UserId
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
                    Password_Hash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                    Created_By = session.User.Entity.Id,
                    Is_Enabled = true
                });
                uow.Commit();
            }

            await CreateOrDeleteUserRoles(request.RoleIds, id, session.User.Entity.Id);

            await _sessionService.WriteSessionLogEvent(new Models.ServiceModels.Session.CreateSessionLogEventRequest()
            {
                EventKey = SessionEventKeys.UserCreated,
                Message = $"User Id: {id}"
            });

            response.Notifications.Add($"User {request.Username} has been created", NotificationTypeEnum.Success);
            return response;
        }

        public async Task<UpdateUserResponse> UpdateUser(UpdateUserRequest request)
        {
            var session = await _sessionService.GetAuthenticatedSession();
            var response = new UpdateUserResponse();

            var duplicateResponse = await _accountService.DuplicateUserCheck(new DuplicateUserCheckRequest()
            {
                EmailAddress = request.EmailAddress,
                Username = request.Username,
                MobileNumber = request.MobileNumber,
                UserId = request.Id
            });

            if (duplicateResponse.Notifications.HasErrors)
            {
                response.Notifications.Add(duplicateResponse.Notifications);
                return response;
            }

            using (var uow = _uowFactory.GetUnitOfWork())
            {
                var user = await uow.UserRepo.GetUserById(new Infrastructure.Repositories.UserRepo.Models.GetUserByIdRequest()
                {
                    Id = request.Id
                });

                var dbRequest = new Infrastructure.Repositories.UserRepo.Models.UpdateUserRequest()
                {
                    Id = request.Id,
                    Username = request.Username,
                    First_Name = request.FirstName,
                    Last_Name = request.LastName,
                    Mobile_Number = request.MobileNumber,
                    Email_Address = request.EmailAddress,
                    Password_Hash = user.Password_Hash,
                    Registration_Confirmed = request.RegistrationConfirmed,
                    Updated_By = session.User.Entity.Id,
                };

                if (!string.IsNullOrEmpty(request.Password))
                {
                    dbRequest.Password_Hash = BCrypt.Net.BCrypt.HashPassword(request.Password);
                }

                await uow.UserRepo.UpdateUser(dbRequest);
                uow.Commit();
            }

            await CreateOrDeleteUserRoles(request.RoleIds, request.Id, session.User.Entity.Id);

            await _sessionService.WriteSessionLogEvent(new Models.ServiceModels.Session.CreateSessionLogEventRequest()
            {
                EventKey = SessionEventKeys.UserUpdated
            });

            response.Notifications.Add($"User {request.Username} has been updated", NotificationTypeEnum.Success);
            return response;
        }

        private async Task CreateOrDeleteUserRoles(List<int> newRoles, int userId, int loggedInUserId)
        {
            var userRoles = await _cache.UserRoles();
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
                _cache.Remove(CacheConstants.UserRoles);

                await _sessionService.WriteSessionLogEvent(new Models.ServiceModels.Session.CreateSessionLogEventRequest()
                {
                    EventKey = SessionEventKeys.UserRolesUpdated
                });
            }
        }

        public async Task<UnlockUserResponse> UnlockUser(UnlockUserRequest request)
        {
            var session = await _sessionService.GetAuthenticatedSession();
            var response = new UnlockUserResponse();

            UserEntity user;
            using (var uow = _uowFactory.GetUnitOfWork())
            {
                user = await uow.UserRepo.GetUserById(new Infrastructure.Repositories.UserRepo.Models.GetUserByIdRequest()
                {
                    Id = request.Id
                });

                await uow.UserRepo.UnlockUser(new Infrastructure.Repositories.UserRepo.Models.UnlockUserRequest()
                {
                    Id = request.Id,
                    Updated_By = session.User.Entity.Id
                });
                uow.Commit();

                await _sessionService.WriteSessionLogEvent(new Models.ServiceModels.Session.CreateSessionLogEventRequest()
                {
                    EventKey = SessionEventKeys.UserUnlocked
                });
            }

            response.Notifications.Add($"User '{user.Username}' has been unlocked", NotificationTypeEnum.Success);
            return response;
        }

        #endregion

        #region Role Management

        public async Task<GetRoleManagementResponse> GetRoleManagement()
        {
            var response = new GetRoleManagementResponse();

            response.Roles = await _cache.Roles();

            return response;
        }

        public async Task<EnableRoleResponse> EnableRole(EnableRoleRequest request)
        {
            var session = await _sessionService.GetAuthenticatedSession();
            var response = new EnableRoleResponse();

            var roles = await _cache.Roles();
            var role = roles.FirstOrDefault(u => u.Id == request.Id);

            using (var uow = _uowFactory.GetUnitOfWork())
            {
                await uow.UserRepo.EnableRole(new Infrastructure.Repositories.UserRepo.Models.EnableRoleRequest()
                {
                    Role_Id = role.Id,
                    Updated_By = session.User.Entity.Id
                });
                uow.Commit();
            }

            _cache.Remove(CacheConstants.Roles);

            await _sessionService.WriteSessionLogEvent(new Models.ServiceModels.Session.CreateSessionLogEventRequest()
            {
                EventKey = SessionEventKeys.RoleEnabled
            });

            response.Notifications.Add($"Role '{role.Name}' has been enabled", NotificationTypeEnum.Success);
            return response;
        }

        public async Task<DisableRoleResponse> DisableRole(DisableRoleRequest request)
        {
            var session = await _sessionService.GetAuthenticatedSession();
            var response = new DisableRoleResponse();

            var roles = await _cache.Roles();
            var role = roles.FirstOrDefault(u => u.Id == request.Id);

            using (var uow = _uowFactory.GetUnitOfWork())
            {
                await uow.UserRepo.DisableRole(new Infrastructure.Repositories.UserRepo.Models.DisableRoleRequest()
                {
                    Id = role.Id,
                    Updated_By = session.User.Entity.Id
                });
                uow.Commit();
            }

            _cache.Remove(CacheConstants.Roles);

            await _sessionService.WriteSessionLogEvent(new Models.ServiceModels.Session.CreateSessionLogEventRequest()
            {
                EventKey = SessionEventKeys.RoleDisabled
            });

            response.Notifications.Add($"Role '{role.Name}' has been disabled", NotificationTypeEnum.Success);
            return response;
        }

        public async Task<GetRoleResponse> GetRole(GetRoleRequest request)
        {
            var response = new GetRoleResponse();

            var roles = await _cache.Roles();
            var permissions = await _cache.Permissions();
            var rolePermissions = await _cache.RolePermissions();

            var role = roles.FirstOrDefault(r => r.Id == request.RoleId);
            if (role == null)
            {
                response.Notifications.AddError($"Could not find role with Id {request.RoleId}");
                return response;
            }

            var rolesPermissions = rolePermissions.Where(rc => rc.Role_Id == request.RoleId).Select(rc => rc.Permission_Id);

            response.Role = role;
            response.Permissions = permissions.Where(c => rolesPermissions.Contains(c.Id)).ToList();

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
                    Created_By = session.User.Entity.Id,
                });
                uow.Commit();
            }

            _cache.Remove(CacheConstants.Roles);

            var roles = await _cache.Roles();
            var role = roles.FirstOrDefault(r => r.Id == id);

            await CreateOrDeleteRolePermissions(request.PermissionIds, id, session.User.Entity.Id);

            await _sessionService.WriteSessionLogEvent(new Models.ServiceModels.Session.CreateSessionLogEventRequest()
            {
                EventKey = SessionEventKeys.RoleCreated
            });

            response.Notifications.Add($"Role '{request.Name}' has been created", NotificationTypeEnum.Success);
            return response;
        }

        public async Task<UpdateRoleResponse> UpdateRole(UpdateRoleRequest request)
        {
            var session = await _sessionService.GetAuthenticatedSession();
            var response = new UpdateRoleResponse();

            var roles = await _cache.Roles();
            var role = roles.FirstOrDefault(u => u.Id == request.Id);

            using (var uow = _uowFactory.GetUnitOfWork())
            {
                await uow.UserRepo.UpdateRole(new Infrastructure.Repositories.UserRepo.Models.UpdateRoleRequest()
                {
                    Id = request.Id,
                    Name = request.Name,
                    Description = request.Description,
                    Updated_By = session.User.Entity.Id
                });
                uow.Commit();
            }

            await CreateOrDeleteRolePermissions(request.PermissionIds, request.Id, session.User.Entity.Id);

            _cache.Remove(CacheConstants.Roles);
            _cache.Remove(CacheConstants.RolePermissions);

            await _sessionService.WriteSessionLogEvent(new Models.ServiceModels.Session.CreateSessionLogEventRequest()
            {
                EventKey = SessionEventKeys.RoleUpdated
            });

            response.Notifications.Add($"Role '{role.Name}' has been updated", NotificationTypeEnum.Success);
            return response;
        }

        private async Task CreateOrDeleteRolePermissions(List<int> newPermissions, int roleId, int loggedInUserId)
        {
            var rolePermissions = await _cache.RolePermissions();
            var existingPermissions = rolePermissions.Where(rc => rc.Role_Id == roleId).Select(rc => rc.Permission_Id);

            var permissionsToBeDeleted = existingPermissions.Where(ur => !newPermissions.Contains(ur));
            var permissionsToBeCreated = newPermissions.Where(ur => !existingPermissions.Contains(ur));

            if (permissionsToBeDeleted.Any() || permissionsToBeCreated.Any())
            {
                using (var uow = _uowFactory.GetUnitOfWork())
                {
                    foreach (var permissionId in permissionsToBeCreated)
                    {
                        await uow.UserRepo.CreateRolePermission(new Infrastructure.Repositories.UserRepo.Models.CreateRolePermissionRequest()
                        {
                            Role_Id = roleId,
                            Permission_Id = permissionId,
                            Created_By = loggedInUserId
                        });

                    }

                    foreach (var permissionId in permissionsToBeDeleted)
                    {
                        await uow.UserRepo.DeleteRolePermission(new Infrastructure.Repositories.UserRepo.Models.DisableRolePermissionRequest()
                        {
                            Role_Id = roleId,
                            Permission_Id = permissionId,
                            Updated_By = loggedInUserId
                        });

                    }
                    uow.Commit();
                }
                _cache.Remove(CacheConstants.RolePermissions);

                await _sessionService.WriteSessionLogEvent(new Models.ServiceModels.Session.CreateSessionLogEventRequest()
                {
                    EventKey = SessionEventKeys.RolePermissionsUpdated
                });
            }
        }

        #endregion

        #region Configuration Management

        public async Task<GetConfigurationManagementResponse> GetConfigurationManagement()
        {
            var response = new GetConfigurationManagementResponse();

            var configuration = await _cache.Configuration();
            response.ConfigurationItems = configuration.Items;

            return response;
        }

        public async Task<GetConfigurationItemResponse> GetConfigurationItem(GetConfigurationItemRequest request)
        {
            var response = new GetConfigurationItemResponse();

            var configuration = await _cache.Configuration();
            var configItem = configuration.Items.FirstOrDefault(c => c.Id == request.Id);

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
                    Date_Value = request.DateValue,
                    Time_Value = request.TimeValue,
                    Decimal_Value = request.DecimalValue,
                    Int_Value = request.IntValue,
                    Money_Value = request.MoneyValue,
                    String_Value = request.StringValue,
                    Updated_By = session.User.Entity.Id
                });
                uow.Commit();
            }

            _cache.Remove(CacheConstants.ConfigurationItems);

            var configuration = await _cache.Configuration();
            var configItem = configuration.Items.FirstOrDefault(c => c.Id == request.Id);

            await _sessionService.WriteSessionLogEvent(new Models.ServiceModels.Session.CreateSessionLogEventRequest()
            {
                EventKey = SessionEventKeys.ConfigurationUpdated
            });

            response.Notifications.Add($"Configuration item '{configItem.Key}' has been updated", NotificationTypeEnum.Success);
            return response;
        }

        public async Task<CreateConfigurationItemResponse> CreateConfigurationItem(CreateConfigurationItemRequest request)
        {
            var session = await _sessionService.GetAuthenticatedSession();
            var response = new CreateConfigurationItemResponse();

            var configuration = await _cache.Configuration();
            var configItem = configuration.Items.FirstOrDefault(c => c.Key == request.Key);

            if (configItem != null)
            {
                response.Notifications.AddError($"A configuration item already exists with the key {request.Key}");
                return response;
            }

            int id;
            using (var uow = _uowFactory.GetUnitOfWork())
            {
                id = await uow.ConfigurationRepo.CreateConfigurationItem(new Infrastructure.Repositories.ConfigurationRepo.Models.CreateConfigurationItemRequest()
                {
                    Key = request.Key,
                    Description = request.Description,
                    Boolean_Value = request.BooleanValue,
                    DateTime_Value = request.DateTimeValue,
                    Date_Value = request.DateValue,
                    Time_Value = request.TimeValue,
                    Decimal_Value = request.DecimalValue,
                    Int_Value = request.IntValue,
                    Money_Value = request.MoneyValue,
                    String_Value = request.StringValue,
                    Created_By = session.User.Entity.Id
                });
                uow.Commit();
            }

            _cache.Remove(CacheConstants.ConfigurationItems);

            await _sessionService.WriteSessionLogEvent(new Models.ServiceModels.Session.CreateSessionLogEventRequest()
            {
                EventKey = SessionEventKeys.ConfigurationCreated
            });

            response.Notifications.Add($"Configuration item '{request.Key}' has been created", NotificationTypeEnum.Success);
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
                var logEvents = await uow.SessionRepo.GetSessionLogEventsBySessionId(new Infrastructure.Repositories.SessionRepo.Models.GetSessionLogEventsBySessionIdRequest()
                {
                    Session_Id = request.Id
                });

                if (session.User_Id.HasValue)
                {
                    var user = await uow.UserRepo.GetUserById(new Infrastructure.Repositories.UserRepo.Models.GetUserByIdRequest()
                    {
                        Id = session.User_Id.Value
                    });
                    response.User = user;
                }

                var eventsLookup = await _cache.SessionEvents();

                response.Session = session;
                response.Logs = logs.Select(l =>
                {

                    var eventIds = logEvents.Where(le => le.Session_Log_Id == l.Id).Select(le => le.Event_Id);
                    return new SessionLog()
                    {
                        Entity = l,
                        Events = eventsLookup.Where(e => eventIds.Contains(e.Id)).Select(e => new SessionLogEvent()
                        {
                            Event = e,
                            Message = logEvents.FirstOrDefault(le => le.Event_Id == e.Id).Message
                        }).ToList()
                    };

                }).ToList();

                uow.Commit();
                return response;
            }
        }

        public async Task<GetSessionsResponse> GetSessions(GetSessionsRequest request)
        {
            var response = new GetSessionsResponse();
            var sessions = new List<Infrastructure.Repositories.SessionRepo.Models.GetSessionsResponse>();

            if (request.Last24Hours.HasValue && request.Last24Hours.Value)
            {
                using (var uow = _uowFactory.GetUnitOfWork())
                {
                    sessions = await uow.SessionRepo.GetSessionsByStartDate(new Infrastructure.Repositories.SessionRepo.Models.GetSessionsByStartDateRequest()
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
                    sessions = await uow.SessionRepo.GetSessionsByStartDate(new Infrastructure.Repositories.SessionRepo.Models.GetSessionsByStartDateRequest()
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
                    sessions = await uow.SessionRepo.GetSessionsByDate(new Infrastructure.Repositories.SessionRepo.Models.GetSessionsByDateRequest()
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
                    sessions = await uow.SessionRepo.GetSessionsByUserId(new Infrastructure.Repositories.SessionRepo.Models.GetSessionsByUserIdRequest()
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

                    sessions = await uow.SessionRepo.GetSessionsByUserId(new Infrastructure.Repositories.SessionRepo.Models.GetSessionsByUserIdRequest()
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

                    sessions = await uow.SessionRepo.GetSessionsByUserId(new Infrastructure.Repositories.SessionRepo.Models.GetSessionsByUserIdRequest()
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

                    sessions = await uow.SessionRepo.GetSessionsByUserId(new Infrastructure.Repositories.SessionRepo.Models.GetSessionsByUserIdRequest()
                    {
                        User_Id = user.Id
                    });

                    uow.Commit();
                }
                response.SelectedFilter = $"Email Address: {request.EmailAddress}";
            }

            response.Sessions = sessions.Select(s => new Session()
            {
                Entity = s,
                Username = s.Username
            }).OrderByDescending(s => s.Entity.Created_Date).ToList();
            return response;
        }

        public async Task<GetSessionEventManagementResponse> GetSessionEventManagement()
        {
            var response = new GetSessionEventManagementResponse();

            response.SessionEvents = await _cache.SessionEvents();

            return response;
        }

        public async Task<GetSessionEventResponse> GetSessionEvent(GetSessionEventRequest request)
        {
            var response = new GetSessionEventResponse();

            var sessionEvents = await _cache.SessionEvents();
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
                    Updated_By = session.User.Entity.Id
                });
                uow.Commit();
            }

            _cache.Remove(CacheConstants.SessionEvents);

            var sessionEvents = await _cache.SessionEvents();
            var sessionEvent = sessionEvents.FirstOrDefault(c => c.Id == request.Id);

            await _sessionService.WriteSessionLogEvent(new Models.ServiceModels.Session.CreateSessionLogEventRequest()
            {
                EventKey = SessionEventKeys.SessionEventUpdated
            });

            response.Notifications.Add($"Session event '{sessionEvent.Key}' has been updated", NotificationTypeEnum.Success);
            return response;
        }

        public async Task<CreateSessionEventResponse> CreateSessionEvent(CreateSessionEventRequest request)
        {
            var session = await _sessionService.GetAuthenticatedSession();
            var response = new CreateSessionEventResponse();

            var sessionEvents = await _cache.SessionEvents();
            var sessionEvent = sessionEvents.FirstOrDefault(se => se.Key == request.Key);

            if (sessionEvent != null)
            {
                response.Notifications.AddError($"A session event already exists with the key {request.Key}");
                return response;
            }

            int id;
            using (var uow = _uowFactory.GetUnitOfWork())
            {
                id = await uow.SessionRepo.CreateSessionEvent(new Infrastructure.Repositories.SessionRepo.Models.CreateSessionEventRequest()
                {
                    Key = request.Key,
                    Description = request.Description,
                    Created_By = session.User.Entity.Id
                });
                uow.Commit();
            }

            _cache.Remove(CacheConstants.SessionEvents);

            await _sessionService.WriteSessionLogEvent(new Models.ServiceModels.Session.CreateSessionLogEventRequest()
            {
                EventKey = SessionEventKeys.SessionEventCreated
            });

            response.Notifications.Add($"Session event '{request.Key}' has been created", NotificationTypeEnum.Success);
            return response;
        }

        #endregion

        #region Permission Management 

        public async Task<GetPermissionManagementResponse> GetPermissionManagement()
        {
            var response = new GetPermissionManagementResponse();

            response.Permissions = await _cache.Permissions();

            return response;
        }

        public async Task<GetPermissionResponse> GetPermission(GetPermissionRequest request)
        {
            var response = new GetPermissionResponse();

            var permissions = await _cache.Permissions();
            var permission = permissions.FirstOrDefault(c => c.Id == request.Id);

            response.Permission = permission;

            return response;
        }

        public async Task<UpdatePermissionResponse> UpdatePermission(UpdatePermissionRequest request)
        {
            var session = await _sessionService.GetAuthenticatedSession();
            var response = new UpdatePermissionResponse();

            using (var uow = _uowFactory.GetUnitOfWork())
            {
                await uow.UserRepo.UpdatePermission(new Infrastructure.Repositories.UserRepo.Models.UpdatePermissionRequest()
                {
                    Id = request.Id,
                    Name = request.Name,
                    Group_Name = request.GroupName,
                    Description = request.Description,
                    Updated_By = session.User.Entity.Id
                });
                uow.Commit();
            }

            _cache.Remove(CacheConstants.Permissions);

            var permissions = await _cache.Permissions();
            var permission = permissions.FirstOrDefault(c => c.Id == request.Id);

            await _sessionService.WriteSessionLogEvent(new Models.ServiceModels.Session.CreateSessionLogEventRequest()
            {
                EventKey = SessionEventKeys.PermissionUpdated
            });

            response.Notifications.Add($"Permission '{permission.Key}' has been updated", NotificationTypeEnum.Success);
            return response;
        }

        public async Task<CreatePermissionResponse> CreatePermission(CreatePermissionRequest request)
        {
            var session = await _sessionService.GetAuthenticatedSession();
            var response = new CreatePermissionResponse();

            var permissions = await _cache.Permissions();
            var permission = permissions.FirstOrDefault(c => c.Key == request.Key);

            if (permission != null)
            {
                response.Notifications.AddError($"A permission already exists with the key {request.Key}");
                return response;
            }

            int id;
            using (var uow = _uowFactory.GetUnitOfWork())
            {
                id = await uow.UserRepo.CreatePermission(new Infrastructure.Repositories.UserRepo.Models.CreatePermissionRequest()
                {
                    Key = request.Key,
                    Description = request.Description,
                    Group_Name = request.GroupName,
                    Name = request.Name,
                    Created_By = session.User.Entity.Id
                });
                uow.Commit();
            }

            _cache.Remove(CacheConstants.Permissions);

            await _sessionService.WriteSessionLogEvent(new Models.ServiceModels.Session.CreateSessionLogEventRequest()
            {
                EventKey = SessionEventKeys.PermissionCreated
            });

            response.Notifications.Add($"Permission '{request.Key}' has been created", NotificationTypeEnum.Success);
            return response;
        }

        #endregion

        #endregion
    }
}
