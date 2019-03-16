using System;
using System.Collections.Generic;
using System.Text;

namespace Template.Infrastructure.Repositories.SessionRepo.Models
{
    public class AddUserToSessionRequest
    {
        public string Guid { get; set; }

        public int User_Id { get; set; }

        public int Updated_By { get; set; }
    }
}
