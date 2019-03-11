namespace Template.Infrastructure.Repositories.UserRepo.Models
{
    public class CreateRoleClaimRequest
    {
        public int Role_Id { get; set; }

        public string Claim_Type { get; set; }

        public string Claim_Value { get; set; }

        public int Created_By { get; set; }
    }
}
