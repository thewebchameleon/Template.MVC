using System;
using System.Collections.Generic;

namespace Template.Models.ViewModels.Admin
{
    public class UserManagementViewModel
    {
        public List<User> Users { get; set; }

        public UserManagementViewModel()
        {
            Users = new List<User>();
        }
    }

    public class User
    {
        public string Username { get; set; }

        public string EmailAddress { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string MobileNumber { get; set; }

        public DateTime RegisteredDate { get; set; }
    }
}
