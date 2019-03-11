using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Template.Infrastructure.Cache;
using Template.Infrastructure.Cache.Contracts;
using Template.Infrastructure.Identity.Contracts;
using Template.Infrastructure.Repositories.UserRepo.Models;
using Template.Infrastructure.UnitOfWork.Contracts;
using Template.Models.DomainModels;

namespace Template.Infrastructure.Identity
{
    public class RoleManager : IRoleManager,
        IRoleStore<Role>,
        IRoleClaimStore<Role>
    {
        #region Instance Fields

        private readonly IEntityCache _entityCache;
        private readonly IUnitOfWorkFactory _uowFactory;

        #endregion

        #region Constructor

        public RoleManager(IEntityCache entityCache, IUnitOfWorkFactory uowFactory)
        {
            _entityCache = entityCache;
            _uowFactory = uowFactory;
        }

        #endregion

        #region Public Methods

        public async Task<IdentityResult> CreateAsync(Role role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using (var uow = _uowFactory.GetUnitOfWork())
            {
                var roleId = await uow.UserRepo.CreateRole(new CreateRoleRequest()
                {
                    Name = role.Name,
                    Description = role.Description,
                    Created_By = role.Created_By
                });
                role.Id = roleId;

                uow.Commit();
            }
            _entityCache.Remove(CacheConstants.Roles);

            return IdentityResult.Success;
        }

        public async Task<IdentityResult> UpdateAsync(Role role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using (var uow = _uowFactory.GetUnitOfWork())
            {
                await uow.UserRepo.UpdateRole(new UpdateRoleRequest()
                {
                    Id = role.Id,
                    Name = role.Name,
                    Description = role.Description,
                    Updated_By = role.Updated_By
                });

                uow.Commit();
            }
            _entityCache.Remove(CacheConstants.Roles);

            return IdentityResult.Success;
        }

        public async Task<IdentityResult> DeleteAsync(Role role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using (var uow = _uowFactory.GetUnitOfWork())
            {
                await uow.UserRepo.DeleteRole(new DeleteRoleRequest()
                {
                    Id = role.Id,
                    Updated_By = role.Updated_By
                });

                uow.Commit();
            }
            _entityCache.Remove(CacheConstants.Roles);

            return IdentityResult.Success;
        }

        public Task<string> GetRoleIdAsync(Role role, CancellationToken cancellationToken)
        {
            return Task.FromResult(role.Id.ToString());
        }

        public Task<string> GetRoleNameAsync(Role role, CancellationToken cancellationToken)
        {
            return Task.FromResult(role.Name);
        }

        public Task SetRoleNameAsync(Role role, string roleName, CancellationToken cancellationToken)
        {
            role.Name = roleName;
            return Task.FromResult(0);
        }

        public Task<string> GetNormalizedRoleNameAsync(Role role, CancellationToken cancellationToken)
        {
            return Task.FromResult(role.Name);
        }

        public Task SetNormalizedRoleNameAsync(Role role, string normalizedName, CancellationToken cancellationToken)
        {
            role.Name = normalizedName;
            return Task.FromResult(0);
        }

        public async Task<Role> FindByIdAsync(string roleId, CancellationToken cancellationToken)
        {
            int.TryParse(roleId, out int id);

            cancellationToken.ThrowIfCancellationRequested();

            var roles = await _entityCache.Roles();
            return roles.FirstOrDefault(r => r.Id == id);
        }

        public async Task<Role> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var roles = await _entityCache.Roles();
            return roles.FirstOrDefault(r => r.Name == normalizedRoleName);
        }

        public void Dispose()
        {
            // Nothing to dispose.
        }

        public async Task<IList<System.Security.Claims.Claim>> GetClaimsAsync(Role role, CancellationToken cancellationToken = default(CancellationToken))
        {
            var claims = await _entityCache.Claims();
            var roleClaims = await _entityCache.RoleClaims();
            roleClaims = roleClaims.Where(rc => rc.RoleId == role.Id && rc.Is_Deleted).ToList();

            var result = new List<System.Security.Claims.Claim>();
            foreach (var claim in roleClaims)
            {
                var claimItem = claims.FirstOrDefault(c => c.Id == claim.Id);
                result.Add(new System.Security.Claims.Claim(claimItem.Type, claimItem.Value));
            }
            return result;
        }

        public async Task AddClaimAsync(Role role, System.Security.Claims.Claim claim, CancellationToken cancellationToken = default(CancellationToken))
        {
            var roleClaims = await _entityCache.RoleClaims();
            roleClaims = roleClaims.Where(rc => rc.RoleId == role.Id).ToList();

            using (var uow = _uowFactory.GetUnitOfWork())
            {
                await uow.UserRepo.CreateRoleClaim(new CreateRoleClaimRequest()
                {
                    Role_Id = role.Id,
                    Claim_Type = claim.Type,
                    Claim_Value = claim.Value,
                    Created_By = role.Updated_By
                });
                uow.Commit();
            }
            _entityCache.Remove(CacheConstants.RoleClaims);
        }

        public async Task RemoveClaimAsync(Role role, System.Security.Claims.Claim claim, CancellationToken cancellationToken = default(CancellationToken))
        {
            var roleClaims = await _entityCache.RoleClaims();
            roleClaims = roleClaims.Where(rc => rc.RoleId == role.Id).ToList();

            using (var uow = _uowFactory.GetUnitOfWork())
            {
                await uow.UserRepo.DeleteRoleClaim(new DeleteRoleClaimRequest()
                {
                    Role_Id = role.Id,
                    Claim_Type = claim.Type,
                    Claim_Value = claim.Value,
                    Updated_By = role.Updated_By
                });
                uow.Commit();
            }
            _entityCache.Remove(CacheConstants.RoleClaims);
        }

        #endregion
    }
}
