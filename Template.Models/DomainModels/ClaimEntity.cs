namespace Template.Models.DomainModels
{
    public class ClaimEntity : BaseEntity
    {
        public string Key { get; set; }

        public string Name { get; set; }

        public string GroupName { get; set; }

        public string Description { get; set; }
    }
}
