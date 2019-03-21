using System.Collections.Generic;
using System.Threading.Tasks;
using Template.Models.DomainModels;

namespace Template.Infrastructure.Cache.Contracts
{
    public interface IEntityCache
    {
        Task<List<ConfigurationEntity>> ConfigurationItems();

        Task<List<RoleEntity>> Roles();

        Task<List<UserRoleEntity>> UserRoles();

        Task<List<RoleClaim>> RoleClaims();

        Task<List<ClaimEntity>> Claims();

        Task<List<TokenEntity>> Tokens();

        Task<List<SessionEventEntity>> SessionEvents();

        void Remove(string key);
    }
}
