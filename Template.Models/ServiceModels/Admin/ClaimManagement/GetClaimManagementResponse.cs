using System.Collections.Generic;
using Template.Models.DomainModels;

namespace Template.Models.ServiceModels.Admin.ClaimManagement
{
    public class GetClaimManagementResponse : ServiceResponse
    {
        public List<ClaimEntity> Claims { get; set; }

        public GetClaimManagementResponse()
        {
            Claims = new List<ClaimEntity>();
        }
    }
}
