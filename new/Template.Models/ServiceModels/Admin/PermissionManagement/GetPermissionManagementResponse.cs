using System.Collections.Generic;
using Template.Models.DomainModels;

namespace Template.Models.ServiceModels.Admin.PermissionManagement
{
    public class GetPermissionManagementResponse : ServiceResponse
    {
        public List<PermissionEntity> Permissions { get; set; }

        public GetPermissionManagementResponse()
        {
            Permissions = new List<PermissionEntity>();
        }
    }
}
