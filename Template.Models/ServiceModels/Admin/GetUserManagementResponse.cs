using System.Collections.Generic;
using Template.Models.DomainModels;

namespace Template.Models.ServiceModels.Admin
{
    public class GetUserManagementResponse
    {
        public List<User> Users { get; set; }

        public GetUserManagementResponse()
        {
            Users = new List<User>();
        }
    }
}
