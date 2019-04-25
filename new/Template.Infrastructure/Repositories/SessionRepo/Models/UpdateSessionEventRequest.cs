namespace Template.Infrastructure.Repositories.SessionRepo.Models
{
    public class UpdateSessionEventRequest
    {
        public int Id { get; set; }

        public string Description { get; set; }

        public int Updated_By { get; set; }
    }
}
