using System.Collections.Generic;
using Template.Models.DomainModels;

namespace Template.Models.ServiceModels.Admin
{
    public class GetRoleResponse : ServiceResponse
    {
        public RoleEntity Role { get; set; }

        public List<ClaimEntity> Claims { get; set; }

        public GetRoleResponse()
        {
            Claims = new List<ClaimEntity>();
        }
    }
}
