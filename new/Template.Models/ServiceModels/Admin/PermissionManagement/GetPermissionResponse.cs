using Template.Models.DomainModels;

namespace Template.Models.ServiceModels.Admin.PermissionManagement
{
    public class GetPermissionResponse : ServiceResponse
    {
        public PermissionEntity Permission { get; set; }
    }
}
