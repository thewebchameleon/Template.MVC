namespace Template.Infrastructure.Repositories.UserRepo.Models
{
    public class ActivateAccountRequest
    {
        public string Token { get; set; }

        public int Updated_By { get; set; }
    }
}
