namespace Template.Infrastructure.Repositories.UserRepo.Models
{
    public class CreateRoleClaimRequest
    {
        public int Role_Id { get; set; }

        public int Claim_Id { get; set; }

        public int Created_By { get; set; }
    }
}
