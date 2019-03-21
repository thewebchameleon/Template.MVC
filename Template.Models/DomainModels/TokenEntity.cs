using System;
using System.Collections.Generic;
using System.Text;

namespace Template.Models.DomainModels
{
    public class TokenEntity : BaseEntity
    {
        public int User_Id { get; set; }

        public string Value { get; set; }

        public TokenTypeEnum Type_Id { get; set; }

        public DateTime Expiry_Date { get; set; }
    }
}
