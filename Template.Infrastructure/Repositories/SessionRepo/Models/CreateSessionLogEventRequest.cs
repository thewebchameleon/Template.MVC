namespace Template.Infrastructure.Repositories.SessionRepo.Models
{
    public class CreateSessionLogEventRequest
    {
        public int Session_Log_Id { get; set; }

        public int Event_Id { get; set; }

        public string Message { get; set; }

        public int Created_By { get; set; }
    }
}
