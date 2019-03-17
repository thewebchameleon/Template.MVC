using Template.Models.DomainModels;

namespace Template.Models.ServiceModels.Session
{
    public class GetSessionResponse : ServiceResponse
    {
        public int Id { get; set; }

        public User User { get; set; }
    }
}
