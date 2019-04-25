using Microsoft.AspNetCore.Mvc;
using Template.MVC.Filters;

namespace Template.MVC.Attributes
{
    public class AuthorizePermissionAttribute : TypeFilterAttribute
    {
        public AuthorizePermissionAttribute(string key) : base(typeof(PermissionRequirementFilter))
        {
            Arguments = new object[] { key };
        }
    }
}