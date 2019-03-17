﻿using System.Collections.Generic;
using Template.Models.DomainModels;

namespace Template.Models.ViewModels.Admin
{
    public class RoleManagementViewModel : ViewModel
    {
        public List<Role> Roles { get; set; }

        public RoleManagementViewModel()
        {
            Roles = new List<Role>();
        }
    }
}