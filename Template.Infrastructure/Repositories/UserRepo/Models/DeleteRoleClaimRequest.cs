using System;
using System.Collections.Generic;
using System.Text;

namespace Template.Infrastructure.Repositories.UserRepo.Models
{
    public class DeleteRoleClaimRequest
    {
        public int Role_Id { get; set; }

        public string Claim_Type { get; set; }

        public string Claim_Value { get; set; }

        public int Updated_By { get; set; }
    }
}
