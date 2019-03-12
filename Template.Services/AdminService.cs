using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;
using Template.Infrastructure.Cache.Contracts;
using Template.Infrastructure.UnitOfWork.Contracts;
using Template.Models.DomainModels;
using Template.Models.ServiceModels.Admin;
using Template.Services.Contracts;

namespace Template.Services
{
    public class AdminService : IAdminService
    {
        #region Instance Fields

        private readonly ILogger<AdminService> _logger;

        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly SignInManager<User> _signInManager;

        private readonly IEmailService _emailService;

        private readonly IUnitOfWorkFactory _uowFactory;
        private readonly IEntityCache _entityCache;

        #endregion

        #region Constructor

        public AdminService(
            ILogger<AdminService> logger,
            UserManager<User> userManager,
            RoleManager<Role> roleManager,
            SignInManager<User> signInManager,
            IEmailService emailService,
            IUnitOfWorkFactory uowFactory,
            IEntityCache entityCache)
        {
            _logger = logger;

            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;

            _uowFactory = uowFactory;

            _entityCache = entityCache;
            _emailService = emailService;
        }

        #endregion

        #region Public Methods

        public async Task<ActivateUserResponse> ActivateUser(ActivateUserRequest request)
        {
            var response = new ActivateUserResponse();
            response.Notifications.AddError("I still need to implement this");
            return response;
        }

        public async Task<CreateOrUpdateUserResponse> CreateOrUpdateUser(CreateOrUpdateUserRequest request)
        {
            var response = new CreateOrUpdateUserResponse();
            response.Notifications.AddError("I still need to implement this");
            return response;
        }

        public async Task<DeactivateUserResponse> DeactivateUser(DeactivateUserRequest request)
        {
            var response = new DeactivateUserResponse();
            response.Notifications.AddError("I still need to implement this");
            return response;
        }

        public async Task<GetUserResponse> GetUser(GetUserRequest request)
        {
            var response = new GetUserResponse();

            var users = await _entityCache.Users();
            var user = users.FirstOrDefault(u => u.Id == request.UserId);

            if (user == null)
            {
                response.Notifications.AddError($"Could not find user with Id {request.UserId}");
                return response;
            }
            response.User = user;
            return response;
        }

        public async Task<GetUserManagementResponse> GetUserManagement(GetUserManagementRequest request)
        {
            var response = new GetUserManagementResponse();

            response.Users = await _entityCache.Users();

            return response;
        }

        public async Task<CreateUserResponse> CreateUser(CreateUserRequest request)
        {
            var response = new CreateUserResponse();
            response.Notifications.AddError("I still need to implement this");
            return response;
        }

        public async Task<UpdateUserResponse> UpdateUser(UpdateUserRequest request)
        {
            var response = new UpdateUserResponse();
            response.Notifications.AddError("I still need to implement this");
            return response;
        }

        #endregion
    }
}
