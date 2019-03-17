using System;

namespace Template.Infrastructure.Repositories.ConfigurationRepo.Models
{
    public class UpdateConfigurationItemRequest
    {
        public int Id { get; set; }

        public string Description { get; set; }

        public bool? Boolean_Value { get; set; }

        public DateTime? DateTime_Value { get; set; }

        public decimal? Decimal_Value { get; set; }

        public int? Int_Value { get; set; }

        public decimal? Money_Value { get; set; }

        public string String_Value { get; set; }

        public int Updated_By { get; set; }
    }
}
