using System;
using System.Collections.Generic;
using System.Text;

namespace Template.Infrastructure.Repositories.UserRepo.Models
{
    public class CreateTokenRequest
    {
        public int User_Id { get; set; }

        public string Token { get; set; }

        public int Type_Id { get; set; }

        public DateTime Expiry_Date { get; set; }

        public int Created_By { get; set; }
    }
}
