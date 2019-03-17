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
    [Authorize]
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
        [AllowAnonymous]
        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View(new LoginViewModel());
        }

        [HttpPost]
        [AllowAnonymous]
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
        [AllowAnonymous]
        public IActionResult Register(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View(new RegisterViewModel());
        }

        [HttpPost]
        [AllowAnonymous]
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
        [AllowAnonymous]
        public IActionResult Logout()
        {
            _accountService.Logout();
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        //[HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> ForgotPassword(ForgotPasswordRequest request)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var result = await _accountService.ForgotPassword(request);
        //        if (result.IsSuccessful)
        //        {
        //            // Don't reveal that the user does not exist or is not confirmed
        //            return RedirectToAction(nameof(AccountController.ForgotPasswordConfirmation), "Account");
        //        }
        //    }

        //    // If we got this far, something failed, redisplay form
        //    return View(request);
        //}

        //[HttpGet]
        //[AllowAnonymous]
        //public IActionResult ForgotPasswordConfirmation()
        //{
        //    return View();
        //}

        [HttpGet]
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

            // todo: viewmodels should be stored in session to avoid making extra calls to the service
            var viewModel = new ProfileViewModel();
            var profileResponse = await _accountService.GetProfile();
            viewModel.Roles = profileResponse.Roles;
            return View(viewModel);
        }

        #region External Login

        //[HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public IActionResult ExternalLogin(string provider, string returnUrl = null)
        //{
        //    // Request a redirect to the external login provider.
        //    var redirectUrl = Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl });
        //    var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
        //    return Challenge(properties, provider);
        //}

        //[HttpGet]
        //[AllowAnonymous]
        //public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null, string remoteError = null)
        //{
        //    if (remoteError != null)
        //    {
        //        ModelState.AddModelError(string.Empty, $"Error from external provider: {remoteError}");
        //        return View(nameof(Login));
        //    }
        //    var info = await _signInManager.GetExternalLoginInfoAsync();
        //    if (info == null)
        //    {
        //        return RedirectToAction(nameof(Login));
        //    }

        //    // Sign in the user with this external login provider if the user already has a login.
        //    var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false);
        //    if (result.Succeeded)
        //    {
        //        // Update any authentication tokens if login succeeded
        //        await _signInManager.UpdateExternalAuthenticationTokensAsync(info);

        //        _logger.LogInformation(5, "User logged in with {Name} provider.", info.LoginProvider);
        //        return RedirectToLocal(returnUrl);
        //    }
        //    if (result.RequiresTwoFactor)
        //    {
        //        return RedirectToAction(nameof(SendCode), new { ReturnUrl = returnUrl });
        //    }
        //    if (result.IsLockedOut)
        //    {
        //        return View("Lockout");
        //    }
        //    else
        //    {
        //        // If the user does not have an account, then ask the user to create an account.
        //        ViewData["ReturnUrl"] = returnUrl;
        //        ViewData["ProviderDisplayName"] = info.ProviderDisplayName;
        //        var email = info.Principal.FindFirstValue(ClaimTypes.Email);
        //        return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = email });
        //    }
        //}

        //[HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl = null)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        // Get the information about the user from the external login provider
        //        var info = await _signInManager.GetExternalLoginInfoAsync();
        //        if (info == null)
        //        {
        //            return View("ExternalLoginFailure");
        //        }
        //        var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
        //        var result = await _userManager.CreateAsync(user);
        //        if (result.Succeeded)
        //        {
        //            result = await _userManager.AddLoginAsync(user, info);
        //            if (result.Succeeded)
        //            {
        //                await _signInManager.SignInAsync(user, isPersistent: false);
        //                _logger.LogInformation(6, "User created an account using {Name} provider.", info.LoginProvider);

        //                // Update any authentication tokens as well
        //                await _signInManager.UpdateExternalAuthenticationTokensAsync(info);

        //                return RedirectToLocal(returnUrl);
        //            }
        //        }
        //        AddErrors(result);
        //    }

        //    ViewData["ReturnUrl"] = returnUrl;
        //    return View(model);
        //}

        #endregion

        #endregion
    }
}
