using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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

        public IActionResult UserManagement()
        {
            var viewModel = new UserManagementViewModel();

            //var response = _adminService.GetUserManagement();
            //viewModel.Users = response.Users;

            return View(viewModel);
        }

        #endregion
    }
}
