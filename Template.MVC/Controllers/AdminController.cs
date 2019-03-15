using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;
using Template.Infrastructure.Cache.Contracts;
using Template.Models.ServiceModels.Admin;
using Template.Models.ViewModels.Admin;
using Template.Services.Contracts;

namespace Template.MVC.Controllers
{
    [Authorize]
    public class AdminController : BaseController
    {
        #region Instance Fields

        private readonly IAdminService _adminService;
        private readonly IEntityCache _entityCache;
        private readonly ILogger _logger;

        #endregion

        #region Constructors

        public AdminController(
            IAdminService adminService,
            IEntityCache entityCache,
            ILoggerFactory loggerFactory)
        {
            _adminService = adminService;
            _entityCache = entityCache;
            _logger = loggerFactory.CreateLogger<AdminController>();
        }

        #endregion

        #region Public Methods

        [HttpGet]
        public async Task<IActionResult> UserManagement()
        {
            var viewModel = new UserManagementViewModel();

            var response = await _adminService.GetUserManagement(new GetUserManagementRequest());
            viewModel.Users = response.Users;

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> CreateUser()
        {
            var viewModel = new CreateUserViewModel()
            {
                ClaimsLookup = await _entityCache.Claims(),
                RolesLookup = await _entityCache.Roles()
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser(CreateUserRequest request)
        {
            if (ModelState.IsValid)
            {
                var response = await _adminService.CreateUser(request, User.UserId);
                if (response.IsSuccessful)
                {
                    AddNotifications(response);
                    return RedirectToHome();
                }
                AddFormErrors(response);
            }
            return View(new CreateUserViewModel(request));
        }

        [HttpGet]
        public async Task<IActionResult> EditUser(int id)
        {
            var viewModel = new EditUserViewModel()
            {
                ClaimsLookup = await _entityCache.Claims(),
                RolesLookup = await _entityCache.Roles()
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
                UserId = id,
                Username = response.User.Username,
                EmailAddress = response.User.Email_Address,
                FirstName = response.User.First_Name,
                LastName = response.User.Last_Name,
                MobileNumber = response.User.Mobile_Number,
                RegistrationConfirmed = response.User.Registration_Confirmed,
                Is_Locked_Out = response.User.Is_Locked_Out,
                Lockout_End = response.User.Lockout_End,
                Claims = response.Claims.Select(c => c.Id).ToList(),
                Roles = response.Roles.Select(r => r.Id).ToList()
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> EditUser(int id, UpdateUserRequest request)
        {
            if (ModelState.IsValid)
            {
                request.UserId = id;

                var response = await _adminService.UpdateUser(request, User.UserId);
                if (response.IsSuccessful)
                {
                    AddNotifications(response);
                    return RedirectToHome();
                }
                AddFormErrors(response);
            }
            return View(new EditUserViewModel(request));
        }

        [HttpPost]
        public async Task<IActionResult> DisableUser(DisableUserRequest request)
        {
            if (ModelState.IsValid)
            {
                var response = await _adminService.DisableUser(request, User.UserId);
                // this redirects so we push notification to the redirect
                // todo: maintain state of selected item
                AddNotifications(response);
            }
            return RedirectToAction(nameof(AdminController.UserManagement));
        }

        [HttpPost]
        public async Task<IActionResult> EnableUser(EnableUserRequest request)
        {
            if (ModelState.IsValid)
            {
                var response = await _adminService.EnableUser(request, User.UserId);
                // this redirects so we push notification to the redirect
                // todo: maintain state of selected item
                AddNotifications(response);
            }
            return RedirectToAction(nameof(AdminController.UserManagement));
        }

        #endregion
    }
}
