using System;
using System.Collections.Generic;
using System.Text;

namespace Template.Infrastructure.Repositories.UserRepo.Models
{
    public class DeleteRoleRequest
    {
        public int Id { get; set; }

        public int Updated_By { get; set; }
    }
}
