using System.Linq;
using System.Security.Claims;
using System.Security.Principal;

namespace Template.Infrastructure.Identity
{
    public class ApplicationUser : ClaimsPrincipal
    {
        public ApplicationUser(IPrincipal principal) : base(principal) { }

        public int UserId
        {
            get
            {
                return int.Parse(Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);
            }
        }

        public string Username
        {
            get
            {
                return Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value;
            }
        }

        public string SessionId
        {
            get
            {
                return Claims.FirstOrDefault(c => c.Type == ClaimConstants.SessionId).Value;
            }
        }
    }
}
