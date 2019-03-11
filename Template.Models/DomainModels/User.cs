﻿using System;

namespace Template.Models.DomainModels
{
    public class User : BaseEntity
    {
        public string Username { get; set; }

        public string Email_Address { get; set; }

        public bool Registration_Confirmed { get; set; }

        public string First_Name { get; set; }

        public string Last_Name { get; set; }

        public string Mobile_Number { get; set; }

        public string Password_Hash { get; set; }

        public DateTime? Lockout_End { get; set; }

        public bool Is_Locked_Out { get; set; }
    }
}