using System;

namespace Template.Infrastructure.Repositories.UserRepo.Models
{
    public class AddInvalidLoginAttemptRequest
    {
        public int User_Id { get; set; }

        public DateTime? Lockout_End { get; set; }

        public int Updated_By { get; set; }
    }
}
