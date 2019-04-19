using System.Collections.Generic;
using System.Threading.Tasks;
using Template.Infrastructure.Configuration.Models;
using Template.Models.DomainModels;

namespace Template.Infrastructure.Cache.Contracts
{
    public interface IApplicationCache
    {
        Task<ApplicationConfiguration> Configuration();

        Task<List<RoleEntity>> Roles();

        Task<List<UserRoleEntity>> UserRoles();

        Task<List<RolePermission>> RolePermissions();

        Task<List<PermissionEntity>> Permissions();

        Task<List<SessionEventEntity>> SessionEvents();

        void Remove(string key);
    }
}
