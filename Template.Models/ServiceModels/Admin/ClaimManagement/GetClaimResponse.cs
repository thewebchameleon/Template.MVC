using Template.Models.DomainModels;

namespace Template.Models.ServiceModels.Admin.ClaimManagement
{
    public class GetClaimResponse : ServiceResponse
    {
        public ClaimEntity Claim { get; set; }
    }
}
