using Template.Models.DomainModels;

namespace Template.Models.ServiceModels.Admin
{
    public class GetUserResponse : ServiceResponse
    {
        public User User { get; set; }
    }
}
