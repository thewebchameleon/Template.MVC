namespace Template.Models.DomainModels
{
    public class Role : BaseEntity
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public bool Is_Enabled { get; set; }
    }
}
