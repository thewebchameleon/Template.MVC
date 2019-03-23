using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Template.Models.ViewModels;
using Template.Models.ViewModels.Home;
using Template.Services.Contracts;

namespace Template.MVC.Controllers
{
    public class HomeController : BaseController
    {
        #region Instance Fields

        private readonly IHomeService _homeService;
        private readonly ILogger _logger;

        #endregion

        #region Constructors

        public HomeController(
            IHomeService homeService,
            ILoggerFactory loggerFactory)
        {
            _homeService = homeService;
            _logger = loggerFactory.CreateLogger<HomeController>();
        }

        #endregion

        #region Public Methods

        public async Task<IActionResult> Index()
        {
            var viewModel = new IndexViewModel();

            var response = await _homeService.GetHome();
            viewModel.DisplayPromoBanner = response.DisplayPromoBanner;
            return View(viewModel);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        #endregion
    }
}
