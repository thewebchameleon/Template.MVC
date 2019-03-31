using System.Collections.Generic;
using Template.Models.DomainModels;

namespace Template.Models.ServiceModels.Session
{
    public class GetSessionResponse : ServiceResponse
    {
        public int Id { get; set; }

        public int SessionLogId { get; set; }

        public User User { get; set; }
    }

    public class User
    {
        public UserEntity Entity { get; set; }

        public List<int> RoleIds { get; set; }

        public List<string> PermissionKeys { get; set; }

        public User()
        {
            RoleIds = new List<int>();
            PermissionKeys = new List<string>();
        }
    }
}
