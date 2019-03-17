using Template.Models.DomainModels;

namespace Template.Models.ServiceModels.Admin
{
    public class GetConfigurationItemResponse : ServiceResponse
    {
        public ConfigurationItem ConfigurationItem { get; set; }
    }
}
