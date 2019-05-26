using System;
using System.Collections.Generic;
using System.Text;

namespace Template.Models.DomainModels
{
    public class EmailTemplateEntity : BaseEntity
    {
        public string Key { get; set; }

        public string Description { get; set; }

        public string Body { get; set; }
    }
}
