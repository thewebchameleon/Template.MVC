using Microsoft.AspNetCore.Mvc.Filters;
using System.Threading.Tasks;
using Template.Infrastructure.Configuration;
using Template.Infrastructure.UnitOfWork.Contracts;
using Template.Services.Contracts;

namespace Template.MVC.Filters
{
    public class SessionLoggingFilter : IAsyncActionFilter
    {
        private readonly ISessionService _sessionService;
        private readonly IUnitOfWorkFactory _uowFactory;

        public SessionLoggingFilter(
            ISessionService sessionService,
            IUnitOfWorkFactory uowFactory)
        {
            _sessionService = sessionService;
            _uowFactory = uowFactory;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var session = await _sessionService.GetSession();

            using (var uow = _uowFactory.GetUnitOfWork())
            {
                await uow.SessionRepo.CreateSessionLog(new Infrastructure.Repositories.SessionRepo.Models.CreateSessionLogRequest()
                {
                    Session_Id = session.Id,
                    Method = context.HttpContext.Request.Method,
                    Controller = (string)context.RouteData.Values["Controller"],
                    Action = (string)context.RouteData.Values["Action"],
                    Created_By = ApplicationConstants.SystemUserId
                });
                uow.Commit();
            }

            // do something before the action executes
            var resultContext = await next();
            // do something after the action executes; resultContext.Result will be set
        }
    }
}
