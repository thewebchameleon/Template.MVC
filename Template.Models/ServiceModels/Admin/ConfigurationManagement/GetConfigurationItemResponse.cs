using Template.Models.DomainModels;

namespace Template.Models.ServiceModels.Admin
{
    public class GetConfigurationItemResponse : ServiceResponse
    {
        public ConfigurationEntity ConfigurationItem { get; set; }
    }
}
