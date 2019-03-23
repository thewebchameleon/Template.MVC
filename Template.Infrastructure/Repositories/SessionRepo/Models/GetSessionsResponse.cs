using Template.Models.DomainModels;

namespace Template.Infrastructure.Repositories.SessionRepo.Models
{
    public class GetSessionsResponse : SessionEntity
    {
        public string Username { get; set; }
    }
}
