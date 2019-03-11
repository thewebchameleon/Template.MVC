namespace Template.Infrastructure.Repositories.SessionRepo.Models
{
    public class CreateSessionRequest
    {
        public string Guid { get; set; }

        public int CreatedBy { get; set; }
    }
}
