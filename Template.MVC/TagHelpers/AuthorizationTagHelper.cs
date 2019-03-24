using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Template.Infrastructure.Authentication;

namespace Template.MVC.TagHelpers
{
    [HtmlTargetElement(Attributes = "asp-authorize")]
    [HtmlTargetElement(Attributes = "asp-authorize,asp-permission")]
    public class AuthorizationTagHelper : TagHelper
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthorizationTagHelper(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// Gets or sets the permission name that determines access to the HTML block.
        /// </summary>
        [HtmlAttributeName("asp-permission")]
        public string Permission { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var isAuthenticated = _httpContextAccessor.HttpContext.User.Identity.IsAuthenticated;
            var hasPermission = _httpContextAccessor.HttpContext.User.HasClaim(c =>
                                    c.Type == PermissionConstants.UserPermission
                                    && c.Value == Permission
                                );

            if (!isAuthenticated)
            {
                output.SuppressOutput();
                return;
            }

            if (!string.IsNullOrEmpty(Permission) && !hasPermission)
            {
                output.SuppressOutput();
                return;
            }
        }
    }
}
