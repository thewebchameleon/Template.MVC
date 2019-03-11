using System;

namespace Template.Models.DomainModels
{
    public class ConfigurationItem : BaseEntity
    {
        public string Key { get; set; }

        public string Description { get; set; }

        public bool? Boolean_Value { get; set; }

        public DateTime? DateTime_Value { get; set; }

        public decimal? Decimal_Value { get; set; }

        public int Int_Value { get; set; }

        public decimal? Money_Value { get; set; }

        public string String_Value { get; set; }
    }
}
