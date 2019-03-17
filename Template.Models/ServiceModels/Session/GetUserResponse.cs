using Template.Models.DomainModels;

namespace Template.Models.ServiceModels.Session
{
    public class GetUserResponse : ServiceResponse
    {
        public User User { get; set; }

        public string SessionId { get; set; }
    }
}
