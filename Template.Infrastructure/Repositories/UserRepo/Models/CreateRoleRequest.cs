namespace Template.Infrastructure.Repositories.UserRepo.Models
{
    public class CreateRoleRequest
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public int Created_By { get; set; }
    }
}
