using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Template.Services.Contracts;

namespace Template.MVC.TagHelpers
{
    [HtmlTargetElement(Attributes = "asp-authorize")]
    [HtmlTargetElement(Attributes = "asp-permission")]
    [HtmlTargetElement(Attributes = "asp-permissions")]
    public class AuthorizationTagHelper : TagHelper
    {
        private readonly ISessionService _sessionService;

        public AuthorizationTagHelper(ISessionService sessionService)
        {
            _sessionService = sessionService;
            Permissions = new List<string>();
        }

        /// <summary>
        /// Gets or sets the permission name that determines access to the HTML block.
        /// </summary>
        [HtmlAttributeName("asp-permission")]
        public string Permission { get; set; }

        /// <summary>
        /// Gets or sets the permission name that determines access to the HTML block.
        /// </summary>
        [HtmlAttributeName("asp-permissions")]
        public List<string> Permissions { get; set; }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            var session = await _sessionService.GetSession();
            if (session.User == null)
            {
                output.SuppressOutput();
                return;
            }

            if (!string.IsNullOrEmpty(Permission) && !session.User.PermissionKeys.Contains(Permission))
            {
                output.SuppressOutput();
                return;
            }

            if (Permissions.Any() && !session.User.PermissionKeys.Select(key => Permissions.Contains(key)).Any())
            {
                output.SuppressOutput();
                return;
            }
        }
    }
}
