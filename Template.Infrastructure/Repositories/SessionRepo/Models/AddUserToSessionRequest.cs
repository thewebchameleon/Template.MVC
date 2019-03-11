using System;
using System.Collections.Generic;
using System.Text;

namespace Template.Infrastructure.Repositories.SessionRepo.Models
{
    public class AddUserToSessionRequest
    {
        public string SessionGuid { get; set; }

        public int UserId { get; set; }

        public int UpdatedBy { get; set; }
    }
}
