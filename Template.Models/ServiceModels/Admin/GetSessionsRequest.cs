using System;
using System.ComponentModel.DataAnnotations;

namespace Template.Models.ServiceModels.Admin
{
    public class GetSessionsRequest
    {
        [DataType(DataType.Date)]
        public DateTime? StartDate { get; set; }
    }
}
