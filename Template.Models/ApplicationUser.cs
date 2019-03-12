using System.Linq;
using System.Security.Claims;

namespace Template.Models
{
    public class ApplicationUser
    {
        private readonly ClaimsPrincipal _claimsPrincipal;

        public ApplicationUser(ClaimsPrincipal claimsPrincipal)
        {
            _claimsPrincipal = claimsPrincipal;
        }

        public int UserId
        {
            get
            {
                return int.Parse(_claimsPrincipal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);
            }
        }

        public string Username
        {
            get
            {
                return _claimsPrincipal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value;
            }
        }
    }
}
