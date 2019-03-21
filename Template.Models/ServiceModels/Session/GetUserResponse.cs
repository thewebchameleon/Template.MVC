using Template.Models.DomainModels;

namespace Template.Models.ServiceModels.Session
{
    public class GetUserResponse : ServiceResponse
    {
        public UserEntity User { get; set; }

        public string SessionId { get; set; }
    }
}
