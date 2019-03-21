using System.Collections.Generic;
using Template.Models.DomainModels;

namespace Template.Models.ServiceModels.Admin
{
    public class GetRoleManagementResponse : ServiceResponse
    {
        public List<RoleEntity> Roles { get; set; }

        public GetRoleManagementResponse()
        {
            Roles = new List<RoleEntity>();
        }
    }
}
