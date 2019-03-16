using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Template.Infrastructure.Cache.Contracts;
using Template.Infrastructure.UnitOfWork.Contracts;
using Template.Models.DomainModels;

namespace Template.Infrastructure.Cache
{
    public class EntityCache : IEntityCache
    {
        #region Instance Fields

        private readonly ICacheProvider _cacheProvider;
        private readonly IUnitOfWorkFactory _uowFactory;

        #endregion

        #region Constructor

        public EntityCache(IUnitOfWorkFactory uowFactory, ICacheProvider cacheProvider)
        {
            _uowFactory = uowFactory;
            _cacheProvider = cacheProvider;
        }

        #endregion

        #region Public Methods

        public async Task<List<ConfigurationItem>> ConfigurationItems()
        {
            var items = new List<ConfigurationItem>();
            if (_cacheProvider.TryGet(CacheConstants.ConfigurationItems, out items))
            {
                return items;
            }

            using (var uow = _uowFactory.GetUnitOfWork())
            {
                items = await uow.ConfigurationRepo.GetConfigurationItems();

                uow.Commit();
            }
            _cacheProvider.Set(CacheConstants.ConfigurationItems, items);

            return items;
        }

        public async Task<List<RoleClaim>> RoleClaims()
        {
            var items = new List<RoleClaim>();
            if (_cacheProvider.TryGet(CacheConstants.RoleClaims, out items))
            {
                return items;
            }

            using (var uow = _uowFactory.GetUnitOfWork())
            {
                items = await uow.UserRepo.GetRoleClaims();

                uow.Commit();
            }
            _cacheProvider.Set(CacheConstants.RoleClaims, items);

            return items;
        }

        public async Task<List<Role>> Roles()
        {
            var items = new List<Role>();
            if (_cacheProvider.TryGet(CacheConstants.Roles, out items))
            {
                return items;
            }

            using (var uow = _uowFactory.GetUnitOfWork())
            {
                items = await uow.UserRepo.GetRoles();

                uow.Commit();
            }
            _cacheProvider.Set(CacheConstants.Roles, items);

            return items;
        }

        public async Task<List<Token>> Tokens()
        {
            var items = new List<Token>();
            if (_cacheProvider.TryGet(CacheConstants.Tokens, out items))
            {
                return items;
            }

            using (var uow = _uowFactory.GetUnitOfWork())
            {
                items = await uow.UserRepo.GetTokens();

                uow.Commit();
            }
            _cacheProvider.Set(CacheConstants.Tokens, items);

            return items;
        }

        public async Task<List<UserRole>> UserRoles()
        {
            var items = new List<UserRole>();
            if (_cacheProvider.TryGet(CacheConstants.UserRoles, out items))
            {
                return items;
            }

            using (var uow = _uowFactory.GetUnitOfWork())
            {
                items = await uow.UserRepo.GetUserRoles();

                uow.Commit();
            }
            _cacheProvider.Set(CacheConstants.UserRoles, items);

            return items;
        }

        public async Task<List<Claim>> Claims()
        {
            var items = new List<Claim>();
            if (_cacheProvider.TryGet(CacheConstants.Claims, out items))
            {
                return items;
            }

            using (var uow = _uowFactory.GetUnitOfWork())
            {
                items = await uow.UserRepo.GetClaims();

                uow.Commit();
            }
            _cacheProvider.Set(CacheConstants.Claims, items);

            return items;
        }

        public void Remove(string key)
        {
            _cacheProvider.Remove(key);
        }

        #endregion
    }
}
