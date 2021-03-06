﻿using System;
using System.Collections.Generic;
using System.Text;
using Template.Models.DomainModels;

namespace Template.Models.ServiceModels.Account
{
    public class GetProfileResponse : ServiceResponse
    {
        public string Username { get; set; }

        public string EmailAddress { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string MobileNumber { get; set; }

        public List<RoleEntity> Roles { get; set; }

        public GetProfileResponse()
        {
            Roles = new List<RoleEntity>();
        }
    }
}
