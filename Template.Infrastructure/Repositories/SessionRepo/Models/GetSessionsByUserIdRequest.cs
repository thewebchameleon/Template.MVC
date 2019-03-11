using System;
using System.Collections.Generic;
using System.Text;

namespace Template.Infrastructure.Repositories.SessionRepo.Models
{
    public class GetSessionsByUserIdRequest
    {
        public int UserId { get; set; }
    }
}
