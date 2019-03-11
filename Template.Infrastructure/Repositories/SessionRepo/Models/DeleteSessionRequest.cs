using System;
using System.Collections.Generic;
using System.Text;

namespace Template.Infrastructure.Repositories.SessionRepo.Models
{
    public class DeleteSessionRequest
    {
        public int Id { get; set; }

        public int UpdatedBy { get; set; }
    }
}
