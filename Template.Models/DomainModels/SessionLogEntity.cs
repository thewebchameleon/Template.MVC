namespace Template.Models.DomainModels
{
    public class SessionLogEntity : BaseEntity
    {
        public int Session_Id { get; set; }

        public string Controller { get; set; }

        public string Action { get; set; }
    }
}
