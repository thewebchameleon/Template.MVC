namespace Template.Infrastructure.Repositories.UserRepo.Models
{
    public class DeleteUserClaimRequest
    {
        public int User_Id { get; set; }

        public int Claim_Id { get; set; }

        public int Updated_By { get; set; }
    }
}
