using System;
using System.Collections.Generic;
using System.Text;

namespace Template.Models.ServiceModels.Email
{
    public class SendContactMessageRequest
    {
        public string Name { get; set; }

        public string EmailAddress { get; set; }

        public string Message { get; set; }
    }
}
