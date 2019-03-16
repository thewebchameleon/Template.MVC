using System.Collections.Generic;
using Template.Models.DomainModels;

namespace Template.Models.ServiceModels.Admin
{
    public class GetUserResponse : ServiceResponse
    {
        public User User { get; set; }

        public List<Role> Roles { get; set; }

        public GetUserResponse()
        {
            Roles = new List<Role>();
        }
    }
}
