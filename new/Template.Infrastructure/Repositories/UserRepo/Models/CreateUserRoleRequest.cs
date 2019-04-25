namespace Template.Infrastructure.Repositories.UserRepo.Models
{
    public class CreateUserRoleRequest
    {
        public int User_Id { get; set; }

        public int Role_Id { get; set; }

        public int Created_By { get; set; }
    }
}
