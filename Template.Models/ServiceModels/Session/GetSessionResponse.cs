using Template.Models.DomainModels;

namespace Template.Models.ServiceModels.Session
{
    public class GetSessionResponse : ServiceResponse
    {
        public int Id { get; set; }

        public int SessionLogId { get; set; }

        public UserEntity User { get; set; }
    }
}
