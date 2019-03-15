namespace Template.Models.DomainModels
{
    public class Session : BaseEntity
    {
        public string Guid { get; set; }

        public int? User_Id { get; set; }
    }
}
