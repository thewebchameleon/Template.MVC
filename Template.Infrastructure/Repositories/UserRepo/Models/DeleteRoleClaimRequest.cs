namespace Template.Infrastructure.Repositories.UserRepo.Models
{
    public class DeleteRoleClaimRequest
    {
        public int Role_Id { get; set; }

        public int Claim_Id { get; set; }

        public int Updated_By { get; set; }
    }
}
