using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Template.Infrastructure.Cache.Contracts;
using Template.Models.ServiceModels.Home;
using Template.Services.Contracts;

namespace Template.Services
{
    public class HomeService : IHomeService
    {
        #region Instance Fields

        private readonly ILogger<HomeService> _logger;

        private readonly IApplicationCache _cache;

        #endregion

        #region Constructor

        public HomeService(
            ILogger<HomeService> logger,
            IApplicationCache cache)
        {
            _logger = logger;
            _cache = cache;
        }

        #endregion

        #region Public Methods

        public async Task<GetHomeResponse> GetHome()
        {
            var response = new GetHomeResponse();
            var config = await _cache.Configuration();

            response.DisplayPromoBanner = config.Home_Promo_Banner_Is_Enabled;

            return response;
        }

        #endregion
    }
}
