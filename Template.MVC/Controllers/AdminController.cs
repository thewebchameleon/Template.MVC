using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
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
        private readonly ILogger _logger;

        #endregion

        #region Constructors

        public AdminController(
            IAdminService adminService,
            ILoggerFactory loggerFactory)
        {
            _adminService = adminService;
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
        public IActionResult CreateUser()
        {
            return View(new CreateUserViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser(int id, CreateUserRequest request)
        {
            if (ModelState.IsValid)
            {
                var response = await _adminService.CreateUser(request);
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
            var viewModel = new EditUserViewModel();

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
                Lockout_End = response.User.Lockout_End
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> EditUser(int id, UpdateUserRequest request)
        {
            if (ModelState.IsValid)
            {
                request.UserId = id;

                var response = await _adminService.UpdateUser(request);
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
        public async Task<IActionResult> DeactivateUser(DeactivateUserRequest request)
        {
            if (ModelState.IsValid)
            {
                var response = await _adminService.DeactivateUser(request);
                if (response.IsSuccessful)
                {
                    AddNotifications(response);
                    return RedirectToHome();
                }
                // this redirects so we push notification to the redirect
                // todo: maintain state of selected item
                AddNotifications(response);
            }
            return RedirectToAction(nameof(AdminController.UserManagement));
        }

        #endregion
    }
}
