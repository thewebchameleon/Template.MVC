using System.Collections.Generic;
using Template.Models.DomainModels;

namespace Template.Models.ServiceModels.Admin
{
    public class GetConfigurationManagementResponse
    {
        public List<ConfigurationItem> ConfigurationItems { get; set; }

        public GetConfigurationManagementResponse()
        {
            ConfigurationItems = new List<ConfigurationItem>();
        }
    }
}
