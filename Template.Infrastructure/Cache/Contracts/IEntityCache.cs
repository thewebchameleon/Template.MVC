using System.Collections.Generic;
using System.Threading.Tasks;
using Template.Models.DomainModels;

namespace Template.Infrastructure.Cache.Contracts
{
    public interface IEntityCache
    {
        Task<List<ConfigurationItem>> ConfigurationItems();

        Task<List<Role>> Roles();

        Task<List<UserRole>> UserRoles();

        Task<List<RoleClaim>> RoleClaims();

        Task<List<Claim>> Claims();

        Task<List<User>> Users();

        Task<List<Token>> Tokens();

        void Remove(string key);
    }
}
