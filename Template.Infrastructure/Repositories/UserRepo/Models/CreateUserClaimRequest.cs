namespace Template.Infrastructure.Repositories.UserRepo.Models
{
    public class CreateUserClaimRequest
    {
        public int User_Id { get; set; }

        public int Claim_Id { get; set; }

        public int Created_By { get; set; }
    }
}
