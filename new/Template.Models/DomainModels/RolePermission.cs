namespace Template.Models.DomainModels
{
    public class RolePermission : BaseEntity
    {
        public int Permission_Id { get; set; }

        public int Role_Id { get; set; }
    }
}
