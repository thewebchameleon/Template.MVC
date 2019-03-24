using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using System.Linq;
using System.Threading.Tasks;
using Template.Infrastructure.Cache.Contracts;
using Template.Infrastructure.Configuration;
using Template.Infrastructure.Session;
using Template.Infrastructure.Session.Contracts;
using Template.Infrastructure.UnitOfWork.Contracts;
using Template.Services.Contracts;

namespace Template.MVC.Filters
{
    public class SessionLoggingFilter : IAsyncActionFilter
    {
        private readonly ISessionService _sessionService;
        private readonly IUnitOfWorkFactory _uowFactory;
        private readonly IApplicationCache _cache;
        private readonly ISessionProvider _sessionProvider;

        public SessionLoggingFilter(
            ISessionService sessionService,
            IApplicationCache cache,
            IUnitOfWorkFactory uowFactory,
            ISessionProvider sessionProvider)
        {
            _sessionService = sessionService;
            _uowFactory = uowFactory;
            _cache = cache;
            _sessionProvider = sessionProvider;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            // validate feature is enabled
            var config = await _cache.Configuration();
            if (!config.Session_Logging_Is_Enabled)
            {
                await next();
                return;
            }

            var session = await _sessionService.GetSession();

            int sessionLogId;
            using (var uow = _uowFactory.GetUnitOfWork())
            {
                var dbRequest = new Infrastructure.Repositories.SessionRepo.Models.CreateSessionLogRequest()
                {
                    Session_Id = session.Id,
                    Method = context.HttpContext.Request.Method,
                    Controller = (string)context.RouteData.Values["Controller"],
                    Action = (string)context.RouteData.Values["Action"],
                    Url = context.HttpContext.Request.GetDisplayUrl(),
                    Created_By = ApplicationConstants.SystemUserId
                };

                if (context.ActionArguments.Any())
                {
                    var jsonString = JsonConvert.SerializeObject(context.ActionArguments, Formatting.Indented).Trim();
                    dbRequest.Action_Data_JSON = jsonString;
                }

                sessionLogId = await uow.SessionRepo.CreateSessionLog(dbRequest);
                uow.Commit();

                // required for session event logging
                await _sessionProvider.Set(SessionConstants.SessionLogId, sessionLogId);
            }

            // do something before the action executes
            var resultContext = await next();
            // do something after the action executes; resultContext.Result will be set

            if (resultContext.Exception != null && !resultContext.ExceptionHandled)
            {
                var events = await _cache.SessionEvents();
                var eventItem = events.FirstOrDefault(e => e.Key == SessionEventKeys.Error);

                using (var uow = _uowFactory.GetUnitOfWork())
                {
                    var dbRequest = new Infrastructure.Repositories.SessionRepo.Models.CreateSessionLogEventRequest()
                    {
                        Session_Log_Id = sessionLogId,
                        Event_Id = eventItem.Id,
                        Message = resultContext.Exception.Message,
                        Created_By = ApplicationConstants.SystemUserId
                    };

                    await uow.SessionRepo.CreateSessionLogEvent(dbRequest);
                    uow.Commit();
                }
            }
        }
    }
}
