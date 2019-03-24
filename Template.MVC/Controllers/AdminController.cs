using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;
using Template.Infrastructure.Authentication;
using Template.Infrastructure.Cache.Contracts;
using Template.Models.ServiceModels.Admin;
using Template.Models.ServiceModels.Admin.PermissionManagement;
using Template.Models.ViewModels;
using Template.Models.ViewModels.Admin;
using Template.MVC.Attributes;
using Template.Services.Contracts;

namespace Template.MVC.Controllers
{
    [AuthorizePermission(PermissionKeys.ViewAdmin)]
    public class AdminController : BaseController
    {
        #region Instance Fields

        private readonly IAdminService _adminService;
        private readonly IApplicationCache _cache;
        private readonly ILogger _logger;

        #endregion

        #region Constructors

        public AdminController(
            IAdminService adminService,
            IApplicationCache cache,
            ILoggerFactory loggerFactory)
        {
            _adminService = adminService;
            _cache = cache;
            _logger = loggerFactory.CreateLogger<AdminController>();
        }

        #endregion

        #region Public Methods

        #region User Management

        [HttpGet]
        public async Task<IActionResult> UserManagement()
        {
            var viewModel = new UserManagementViewModel();

            var response = await _adminService.GetUserManagement();
            viewModel.Users = response.Users;

            return View(viewModel);
        }

        [HttpGet]
        [AuthorizePermission(PermissionKeys.ManageUsers)]
        public async Task<IActionResult> CreateUser()
        {
            var roles = await _cache.Roles();
            var viewModel = new CreateUserViewModel()
            {
                RolesLookup = roles.Select(r => new SelectListItem(r.Name, r.Id)).ToList()
            };

            return View(viewModel);
        }

        [HttpPost]
        [AuthorizePermission(PermissionKeys.ManageUsers)]
        public async Task<IActionResult> CreateUser(CreateUserRequest request)
        {
            if (ModelState.IsValid)
            {
                var response = await _adminService.CreateUser(request);
                if (response.IsSuccessful)
                {
                    AddNotifications(response);
                    return RedirectToAction(nameof(AdminController.UserManagement));
                }
                AddFormErrors(response);
            }
            var roles = await _cache.Roles();
            var viewModel = new CreateUserViewModel(request)
            {
                RolesLookup = roles.Select(r => new SelectListItem(r.Name, r.Id)).ToList()
            };
            return View(viewModel);
        }

        [HttpGet]
        [AuthorizePermission(PermissionKeys.ManageUsers)]
        public async Task<IActionResult> EditUser(int id)
        {
            var roles = await _cache.Roles();
            var viewModel = new EditUserViewModel()
            {
                RolesLookup = roles.Select(r => new SelectListItem(r.Name, r.Id)).ToList()
            };

            var response = await _adminService.GetUser(new GetUserRequest()
            {
                UserId = id
            });

            if (!response.IsSuccessful)
            {
                AddNotifications(response);
                return View(viewModel);
            }

            viewModel.Request = new UpdateUserRequest()
            {
                Id = id,
                Username = response.User.Username,
                EmailAddress = response.User.Email_Address,
                FirstName = response.User.First_Name,
                LastName = response.User.Last_Name,
                MobileNumber = response.User.Mobile_Number,
                RegistrationConfirmed = response.User.Registration_Confirmed,
                Is_Locked_Out = response.User.Is_Locked_Out,
                Lockout_End = response.User.Lockout_End,
                RoleIds = response.Roles.Select(r => r.Id).ToList()
            };

            return View(viewModel);
        }

        [HttpPost]
        [AuthorizePermission(PermissionKeys.ManageUsers)]
        public async Task<IActionResult> EditUser(UpdateUserRequest request)
        {
            if (ModelState.IsValid)
            {
                var response = await _adminService.UpdateUser(request);
                if (response.IsSuccessful)
                {
                    AddNotifications(response);
                    return RedirectToAction(nameof(AdminController.UserManagement));
                }
                AddFormErrors(response);
            }

            var roles = await _cache.Roles();
            var viewModel = new EditUserViewModel(request)
            {
                RolesLookup = roles.Select(r => new SelectListItem(r.Name, r.Id)).ToList()
            };
            return View(viewModel);
        }

        [HttpPost]
        [AuthorizePermission(PermissionKeys.ManageUsers)]
        public async Task<IActionResult> DisableUser(DisableUserRequest request)
        {
            if (ModelState.IsValid)
            {
                var response = await _adminService.DisableUser(request);
                AddNotifications(response);
            }
            return RedirectToAction(nameof(AdminController.UserManagement));
        }

        [HttpPost]
        [AuthorizePermission(PermissionKeys.ManageUsers)]
        public async Task<IActionResult> EnableUser(EnableUserRequest request)
        {
            if (ModelState.IsValid)
            {
                var response = await _adminService.EnableUser(request);
                AddNotifications(response);
            }
            return RedirectToAction(nameof(AdminController.UserManagement));
        }

        #endregion

        #region Role Management

        [HttpGet]
        public async Task<IActionResult> RoleManagement()
        {
            var viewModel = new RoleManagementViewModel();

            var response = await _adminService.GetRoleManagement();
            viewModel.Roles = response.Roles;

            return View(viewModel);
        }

        [HttpGet]
        [AuthorizePermission(PermissionKeys.ManageRoles)]
        public async Task<IActionResult> CreateRole()
        {
            var permissions = await _cache.Permissions();
            var viewModel = new CreateRoleViewModel()
            {
                PermissionsLookup = permissions.Select(c => new SelectListItem(c.Name, c.Id, c.Group_Name)).ToList()
            };

            return View(viewModel);
        }

        [HttpPost]
        [AuthorizePermission(PermissionKeys.ManageRoles)]
        public async Task<IActionResult> CreateRole(CreateRoleRequest request)
        {
            var permissions = await _cache.Permissions();
            var viewModel = new CreateRoleViewModel(request)
            {
                PermissionsLookup = permissions.Select(c => new SelectListItem(c.Name, c.Id, c.Group_Name)).ToList()
            };

            if (ModelState.IsValid)
            {
                var response = await _adminService.CreateRole(request);
                if (response.IsSuccessful)
                {
                    AddNotifications(response);
                    return RedirectToAction(nameof(AdminController.RoleManagement));
                }
                AddFormErrors(response);
            }
            return View(viewModel);
        }

        [HttpGet]
        [AuthorizePermission(PermissionKeys.ManageRoles)]
        public async Task<IActionResult> EditRole(int id)
        {
            var permissions = await _cache.Permissions();
            var viewModel = new EditRoleViewModel()
            {
                PermissionsLookup = permissions.Select(c => new SelectListItem(c.Name, c.Id, c.Group_Name)).ToList()
            };

            var response = await _adminService.GetRole(new GetRoleRequest()
            {
                RoleId = id
            });

            if (!response.IsSuccessful)
            {
                AddNotifications(response);
                return View(viewModel);
            }

            viewModel.Request = new UpdateRoleRequest()
            {
                Id = id,
                Name = response.Role.Name,
                Description = response.Role.Description,
                PermissionIds = response.Permissions.Select(c => c.Id).ToList()
            };

            return View(viewModel);
        }

        [HttpPost]
        [AuthorizePermission(PermissionKeys.ManageRoles)]
        public async Task<IActionResult> EditRole(UpdateRoleRequest request)
        {
            if (ModelState.IsValid)
            {
                var response = await _adminService.UpdateRole(request);
                if (response.IsSuccessful)
                {
                    AddNotifications(response);
                    return RedirectToAction(nameof(AdminController.RoleManagement));
                }
                AddFormErrors(response);
            }
            return View(new EditRoleViewModel(request));
        }

        [HttpPost]
        [AuthorizePermission(PermissionKeys.ManageRoles)]
        public async Task<IActionResult> DisableRole(DisableRoleRequest request)
        {
            if (ModelState.IsValid)
            {
                var response = await _adminService.DisableRole(request);
                AddNotifications(response);
            }
            return RedirectToAction(nameof(AdminController.RoleManagement));
        }

        [HttpPost]
        [AuthorizePermission(PermissionKeys.ManageRoles)]
        public async Task<IActionResult> EnableRole(EnableRoleRequest request)
        {
            if (ModelState.IsValid)
            {
                var response = await _adminService.EnableRole(request);
                AddNotifications(response);
            }
            return RedirectToAction(nameof(AdminController.RoleManagement));
        }

        #endregion

        #region Permission Management

        [HttpGet]
        public async Task<IActionResult> PermissionManagement()
        {
            var viewModel = new PermissionManagementViewModel();

            var response = await _adminService.GetPermissionManagement();
            viewModel.Permissions = response.Permissions;

            return View(viewModel);
        }

        [HttpGet]
        [AuthorizePermission(PermissionKeys.ManagePermissions)]
        public IActionResult CreatePermission()
        {
            var viewModel = new CreatePermissionViewModel();
            return View(viewModel);
        }

        [HttpPost]
        [AuthorizePermission(PermissionKeys.ManagePermissions)]
        public async Task<IActionResult> CreatePermission(CreatePermissionRequest request)
        {
            if (ModelState.IsValid)
            {
                var response = await _adminService.CreatePermission(request);
                if (response.IsSuccessful)
                {
                    AddNotifications(response);
                    return RedirectToAction(nameof(AdminController.PermissionManagement));
                }
                AddFormErrors(response);
            }
            var viewModel = new CreatePermissionViewModel(request);
            return View(viewModel);
        }

        [HttpGet]
        [AuthorizePermission(PermissionKeys.ManagePermissions)]
        public async Task<IActionResult> EditPermission(int id)
        {
            var viewModel = new EditPermissionViewModel();

            var response = await _adminService.GetPermission(new GetPermissionRequest()
            {
                Id = id
            });

            if (!response.IsSuccessful)
            {
                AddNotifications(response);
                return View(viewModel);
            }

            viewModel.Key = response.Permission.Key;
            viewModel.Request = new UpdatePermissionRequest()
            {
                Id = response.Permission.Id,
                Name = response.Permission.Name,
                Description = response.Permission.Description,
                GroupName = response.Permission.Group_Name,
            };

            return View(viewModel);
        }

        [HttpPost]
        [AuthorizePermission(PermissionKeys.ManagePermissions)]
        public async Task<IActionResult> EditPermission(UpdatePermissionRequest request)
        {
            if (ModelState.IsValid)
            {
                var response = await _adminService.UpdatePermission(request);
                if (response.IsSuccessful)
                {
                    AddNotifications(response);
                    return RedirectToAction(nameof(AdminController.PermissionManagement));
                }
                AddFormErrors(response);
            }
            var viewModel = new EditPermissionViewModel(request);
            return View(viewModel);
        }


        #endregion

        #region Configuration Management

        [HttpGet]
        public async Task<IActionResult> ConfigurationManagement()
        {
            var viewModel = new ConfigurationManagementViewModel();

            var response = await _adminService.GetConfigurationManagement();
            viewModel.ConfigurationItems = response.ConfigurationItems;

            return View(viewModel);
        }

        [HttpGet]
        [AuthorizePermission(PermissionKeys.ManageConfiguration)]
        public IActionResult CreateConfigurationItem()
        {
            var viewModel = new CreateConfigurationItemViewModel();
            return View(viewModel);
        }

        [HttpPost]
        [AuthorizePermission(PermissionKeys.ManageConfiguration)]
        public async Task<IActionResult> CreateConfigurationItem(CreateConfigurationItemRequest request)
        {
            if (ModelState.IsValid)
            {
                var response = await _adminService.CreateConfigurationItem(request);
                if (response.IsSuccessful)
                {
                    AddNotifications(response);
                    return RedirectToAction(nameof(AdminController.ConfigurationManagement));
                }
                AddFormErrors(response);
            }
            var viewModel = new CreateConfigurationItemViewModel(request);
            return View(viewModel);
        }

        [HttpGet]
        [AuthorizePermission(PermissionKeys.ManageConfiguration)]
        public async Task<IActionResult> EditConfigurationItem(int id)
        {
            var viewModel = new EditConfigurationItemViewModel();

            var response = await _adminService.GetConfigurationItem(new GetConfigurationItemRequest()
            {
                Id = id
            });

            if (!response.IsSuccessful)
            {
                AddNotifications(response);
                return View(viewModel);
            }

            viewModel.Key = response.ConfigurationItem.Key;
            viewModel.Request = new UpdateConfigurationItemRequest()
            {
                Id = response.ConfigurationItem.Id,
                Description = response.ConfigurationItem.Description,
                BooleanValue = response.ConfigurationItem.Boolean_Value,
                DateTimeValue = response.ConfigurationItem.DateTime_Value,
                DecimalValue = response.ConfigurationItem.Decimal_Value,
                IntValue = response.ConfigurationItem.Int_Value,
                MoneyValue = response.ConfigurationItem.Money_Value,
                StringValue = response.ConfigurationItem.String_Value,
            };

            return View(viewModel);
        }

        [HttpPost]
        [AuthorizePermission(PermissionKeys.ManageConfiguration)]
        public async Task<IActionResult> EditConfigurationItem(UpdateConfigurationItemRequest request)
        {
            if (ModelState.IsValid)
            {
                var response = await _adminService.UpdateConfigurationItem(request);
                if (response.IsSuccessful)
                {
                    AddNotifications(response);
                    return RedirectToAction(nameof(AdminController.ConfigurationManagement));
                }
                AddFormErrors(response);
            }
            var viewModel = new EditConfigurationItemViewModel(request);
            return View(viewModel);
        }

        #endregion

        #region Session Event Management

        [HttpGet]
        public async Task<IActionResult> SessionEventManagement()
        {
            var viewModel = new SessionEventManagementViewModel();

            var response = await _adminService.GetSessionEventManagement();
            viewModel.SessionEvents = response.SessionEvents;

            return View(viewModel);
        }

        [HttpGet]
        [AuthorizePermission(PermissionKeys.ManageSessionEvents)]
        public IActionResult CreateSessionEvent()
        {
            var viewModel = new CreateSessionEventViewModel();
            return View(viewModel);
        }

        [HttpPost]
        [AuthorizePermission(PermissionKeys.ManageSessionEvents)]
        public async Task<IActionResult> CreateSessionEvent(CreateSessionEventRequest request)
        {
            if (ModelState.IsValid)
            {
                var response = await _adminService.CreateSessionEvent(request);
                if (response.IsSuccessful)
                {
                    AddNotifications(response);
                    return RedirectToAction(nameof(AdminController.SessionEventManagement));
                }
                AddFormErrors(response);
            }
            var viewModel = new CreateSessionEventViewModel(request);
            return View(viewModel);
        }

        [HttpGet]
        [AuthorizePermission(PermissionKeys.ManageSessionEvents)]
        public async Task<IActionResult> EditSessionEvent(int id)
        {
            var viewModel = new EditSessionEventViewModel();

            var response = await _adminService.GetSessionEvent(new GetSessionEventRequest()
            {
                Id = id
            });

            if (!response.IsSuccessful)
            {
                AddNotifications(response);
                return View(viewModel);
            }

            viewModel.Key = response.SessionEvent.Key;
            viewModel.Request = new UpdateSessionEventRequest()
            {
                Description = response.SessionEvent.Description
            };

            return View(viewModel);
        }

        [HttpPost]
        [AuthorizePermission(PermissionKeys.ManageSessionEvents)]
        public async Task<IActionResult> EditSessionEvent(UpdateSessionEventRequest request)
        {
            if (ModelState.IsValid)
            {
                var response = await _adminService.UpdateSessionEvent(request);
                if (response.IsSuccessful)
                {
                    AddNotifications(response);
                    return RedirectToAction(nameof(AdminController.SessionEventManagement));
                }
                AddFormErrors(response);
            }
            var viewModel = new EditSessionEventViewModel(request);
            return View(viewModel);
        }


        #endregion

        #region Sessions

        [HttpGet]
        [AuthorizePermission(PermissionKeys.ViewSessions)]
        public async Task<IActionResult> Sessions()
        {
            var viewModel = new SessionsViewModel();

            var response = await _adminService.GetSessions(new GetSessionsRequest());
            viewModel.Sessions = response.Sessions;

            return View(viewModel);
        }

        [HttpPost]
        [AuthorizePermission(PermissionKeys.ViewSessions)]
        public async Task<IActionResult> Sessions(GetSessionsRequest request)
        {
            var viewModel = new SessionsViewModel(request);
            if (ModelState.IsValid)
            {
                var response = await _adminService.GetSessions(request);
                if (response.IsSuccessful)
                {
                    viewModel.Sessions = response.Sessions;
                    viewModel.SelectedFilter = response.SelectedFilter;
                    return View(viewModel);
                }
            }
            AddNotifications(request);
            return View(viewModel);
        }

        [HttpGet]
        [AuthorizePermission(PermissionKeys.ViewSessions)]
        public async Task<IActionResult> Session(int id)
        {
            var viewModel = new SessionViewModel();
            var response = await _adminService.GetSession(new GetSessionRequest()
            {
                Id = id
            });

            if (response.IsSuccessful)
            {
                viewModel.User = response.User;
                viewModel.Session = response.Session;
                viewModel.Logs = response.Logs;
            };
            AddNotifications(response);
            return View(viewModel);
        }

        #endregion

        #endregion
    }
}

