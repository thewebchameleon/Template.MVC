using System.Collections.Generic;
using System.Threading.Tasks;
using Template.Models.DomainModels;

namespace Template.Infrastructure.Repositories.ConfigurationRepo.Contracts
{
    public interface IConfigurationRepo
    {
        Task<List<ConfigurationItem>> GetConfigurationItems();
    }
}
