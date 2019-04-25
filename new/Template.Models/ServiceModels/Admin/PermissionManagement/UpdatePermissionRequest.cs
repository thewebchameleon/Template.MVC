﻿using System.ComponentModel.DataAnnotations;

namespace Template.Models.ServiceModels.Admin.PermissionManagement
{
    public class UpdatePermissionRequest
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Group Name")]
        public string GroupName { get; set; }

        [Required]
        public string Name { get; set; }

        public string Description { get; set; }
    }
}
