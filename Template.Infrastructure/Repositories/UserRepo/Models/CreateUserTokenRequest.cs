using System;
using System.Collections.Generic;
using System.Text;

namespace Template.Infrastructure.Repositories.UserRepo.Models
{
    public class CreateUserTokenRequest
    {
        public int User_Id { get; set; }

        public Guid Token { get; set; }

        public int Type_Id { get; set; }

        public int Created_By { get; set; }
    }
}
