namespace Template.Infrastructure.Repositories.SessionRepo.Models
{
    public class CreateSessionLogRequest
    {
        public int Session_Id { get; set; }

        public string Method { get; set; }

        public string Controller { get; set; }

        public string Action { get; set; }

        public string Action_Data_JSON { get; set; }

        public string Url { get; set; }

        public int Created_By { get; set; }
    }
}
