using System.Collections.Generic;
using System.Threading.Tasks;
using Template.Infrastructure.Repositories.ConfigurationRepo.Models;
using Template.Models.DomainModels;

namespace Template.Infrastructure.Repositories.ConfigurationRepo.Contracts
{
    public interface IConfigurationRepo
    {
        Task<List<ConfigurationEntity>> GetConfigurationItems();

        Task UpdateConfigurationItem(UpdateConfigurationItemRequest request);

        Task<int> CreateConfigurationItem(CreateConfigurationItemRequest request);
    }
}
