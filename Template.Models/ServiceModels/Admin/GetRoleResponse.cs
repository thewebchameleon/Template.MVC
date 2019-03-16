using System.Collections.Generic;
using Template.Models.DomainModels;

namespace Template.Models.ServiceModels.Admin
{
    public class GetRoleResponse : ServiceResponse
    {
        public Role Role { get; set; }

        public List<Claim> Claims { get; set; }

        public GetRoleResponse()
        {
            Claims = new List<Claim>();
        }
    }
}
