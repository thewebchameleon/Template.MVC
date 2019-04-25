using System.Collections.Generic;
using Template.Models.DomainModels;

namespace Template.Models.ServiceModels.Admin
{
    public class GetRoleResponse : ServiceResponse
    {
        public RoleEntity Role { get; set; }

        public List<PermissionEntity> Permissions { get; set; }

        public GetRoleResponse()
        {
            Permissions = new List<PermissionEntity>();
        }
    }
}
