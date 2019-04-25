using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Template.Models.ServiceModels;
using Template.Models.ServiceModels.Account;
using Template.Models.ViewModels.Account;
using Template.Services.Contracts;

namespace Template.MVC.Controllers
{
    public class AccountController : BaseController
    {
        #region Instance Fields

        private readonly IAccountService _accountService;
        private readonly ISessionService _sessionService;
        private readonly ILogger _logger;

        #endregion

        #region Constructors

        public AccountController(
            IAccountService accountService,
            ISessionService sessionService,
            ILoggerFactory loggerFactory)
        {
            _accountService = accountService;
            _sessionService = sessionService;
            _logger = loggerFactory.CreateLogger<AccountController>();
        }

        #endregion

        #region Public Methods

        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View(new LoginViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginRequest request, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                var response = await _accountService.Login(request);
                if (response.IsSuccessful)
                {
                    return RedirectToHome(returnUrl);
                }
                AddFormErrors(response);
            }

            // If we got this far, something failed, redisplay form
            return View(new LoginViewModel(request));
        }

        [HttpGet]
        public IActionResult Register(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View(new RegisterViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterRequest request, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                var response = await _accountService.Register(request);
                if (response.IsSuccessful)
                {
                    AddNotifications(response);
                    return RedirectToHome(returnUrl);
                }
                AddFormErrors(response);
            }

            // If we got this far, something failed, redisplay form
            return View(new RegisterViewModel(request));
        }

        [HttpGet]
        public IActionResult Logout()
        {
            _accountService.Logout();
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View(new ForgotPasswordViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordRequest request)
        {
            if (ModelState.IsValid)
            {
                var response = await _accountService.ForgotPassword(request);
                if (response.IsSuccessful)
                {
                    AddNotifications(response);
                    return RedirectToHome();
                }
                AddFormErrors(response);
            }

            // If we got this far, something failed, redisplay form
            return View(new ForgotPasswordViewModel(request));
        }

        [HttpGet]
        public async Task<IActionResult> ActivateAccount([FromQuery] string token)
        {
            var response = await _accountService.ActivateAccount(new ActivateAccountRequest()
            {
                Token = token
            });

            AddNotifications(response);
            return RedirectToHome();
        }

        [HttpGet]
        public async Task<IActionResult> ResetPassword([FromQuery] string token)
        {
            var response = await _accountService.ValidateResetPasswordToken(new ValidateResetPasswordTokenRequest()
            {
                Token = token
            });

            if (response.IsSuccessful)
            {
                var viewModel = new ResetPasswordViewModel();
                viewModel.Request = new ResetPasswordRequest()
                {
                    Token = token
                };

                return View(viewModel);
            }
            AddNotifications(response);
            return RedirectToHome();
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordRequest request)
        {
            if (ModelState.IsValid)
            {
                var response = await _accountService.ResetPassword(request);
                if (response.IsSuccessful)
                {
                    AddNotifications(response);
                    return RedirectToLogin();
                }
                AddFormErrors(response);
            }

            // If we got this far, something failed, redisplay form
            return View(new ResetPasswordViewModel(request));
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Profile()
        {
            var viewModel = new ProfileViewModel();
            var response = await _accountService.GetProfile();

            viewModel.Request = new UpdateProfileRequest()
            {
                EmailAddress = response.EmailAddress,
                FirstName = response.FirstName,
                LastName = response.LastName,
                MobileNumber = response.MobileNumber,
                Username = response.Username
            };
            viewModel.Roles = response.Roles;

            return View(viewModel);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile(UpdateProfileRequest request)
        {
            if (ModelState.IsValid)
            {
                var response = await _accountService.UpdateProfile(request);
                if (response.IsSuccessful)
                {
                    AddNotifications(response);
                    return RedirectToHome();
                }
                AddFormErrors(response);
            }

            var viewModel = new ProfileViewModel();
            var profileResponse = await _accountService.GetProfile();
            viewModel.Roles = profileResponse.Roles;
            return View(viewModel);
        }

        #endregion
    }
}
