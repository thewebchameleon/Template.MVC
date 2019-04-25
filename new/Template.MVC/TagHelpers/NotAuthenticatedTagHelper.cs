using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Threading.Tasks;
using Template.Services.Contracts;

namespace Template.MVC.TagHelpers
{
    [HtmlTargetElement(Attributes = "asp-not-authenticated")]
    public class NotAuthenticatedTagHelper : TagHelper
    {
        private readonly ISessionService _sessionService;

        public NotAuthenticatedTagHelper(ISessionService sessionService)
        {
            _sessionService = sessionService;
        }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            var session = await _sessionService.GetSession();
            if (session.User == null)
            {
                return;
            }
            output.SuppressOutput();
            return;
        }
    }
}
