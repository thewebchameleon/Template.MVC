using Microsoft.AspNetCore.Identity;
using System;
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
    public class UserManager : IUserManager,
        IUserStore<User>,
        IUserRoleStore<User>,
        IUserPasswordStore<User>,
        IUserEmailStore<User>
    {
        #region Instance Fields

        private readonly IEntityCache _entityCache;
        private readonly IUnitOfWorkFactory _uowFactory;

        #endregion

        #region Constructor

        public UserManager(IEntityCache entityCache, IUnitOfWorkFactory uowFactory)
        {
            _entityCache = entityCache;
            _uowFactory = uowFactory;
        }

        #endregion

        #region Public Methods

        public async Task<IdentityResult> CreateAsync(User user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using (var uow = _uowFactory.GetUnitOfWork())
            {
                var userId = await uow.UserRepo.CreateUser(new CreateUserRequest()
                {
                    Username = user.Username,
                    Email_Address = user.Email_Address,
                    Registration_Confirmed = user.Registration_Confirmed,
                    First_Name = user.First_Name,
                    Last_Name = user.Last_Name,
                    Mobile_Number = user.Mobile_Number,
                    Password_Hash = user.Password_Hash,
                    Lockout_End = user.Lockout_End,
                    Is_Locked_Out = user.Is_Locked_Out,
                    Created_By = user.Created_By,
                });
                user.Id = userId;

                uow.Commit();
            }
            _entityCache.Remove(CacheConstants.Users);
            return IdentityResult.Success;
        }

        public async Task<IdentityResult> DeleteAsync(User user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using (var uow = _uowFactory.GetUnitOfWork())
            {
                await uow.UserRepo.DeleteUser(new DeleteUserRequest()
                {
                    Id = user.Id,
                    Updated_By = user.Updated_By
                });

                uow.Commit();
            }
            _entityCache.Remove(CacheConstants.Users);
            return IdentityResult.Success;
        }

        public async Task<User> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            int.TryParse(userId, out int id);

            cancellationToken.ThrowIfCancellationRequested();

            var users = await _entityCache.Users();
            return users.FirstOrDefault(u => u.Id == id);
        }

        public async Task<User> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var users = await _entityCache.Users();
            return users.FirstOrDefault(u => u.Username == normalizedUserName);
        }

        public Task<string> GetNormalizedUserNameAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Username);
        }

        public Task<string> GetUserIdAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Id.ToString());
        }

        public Task<string> GetUserNameAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Username);
        }

        public Task SetNormalizedUserNameAsync(User user, string normalizedName, CancellationToken cancellationToken)
        {
            user.Username = normalizedName;
            return Task.FromResult(0);
        }

        public Task SetUserNameAsync(User user, string userName, CancellationToken cancellationToken)
        {
            user.Username = userName;
            return Task.FromResult(0);
        }

        public async Task<IdentityResult> UpdateAsync(User user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using (var uow = _uowFactory.GetUnitOfWork())
            {
                await uow.UserRepo.UpdateUser(new UpdateUserRequest()
                {
                    Id = user.Id,
                    Username = user.Username,
                    Email = user.Email_Address,
                    Registration_Confirmed = user.Registration_Confirmed,
                    First_Name = user.First_Name,
                    Last_Name = user.Last_Name,
                    Mobile_Number = user.Mobile_Number,
                    Password_Hash = user.Password_Hash,
                    Lockout_End = user.Lockout_End,
                    Is_Locked_Out = user.Is_Locked_Out,
                    Updated_By = user.Updated_By
                });

                uow.Commit();
            }
            _entityCache.Remove(CacheConstants.Users);
            return IdentityResult.Success;
        }

        public Task SetEmailAsync(User user, string email, CancellationToken cancellationToken)
        {
            user.Email_Address = email;
            return Task.FromResult(0);
        }

        public Task<string> GetEmailAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Email_Address);
        }

        public Task<bool> GetEmailConfirmedAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Registration_Confirmed);
        }

        public Task SetEmailConfirmedAsync(User user, bool confirmed, CancellationToken cancellationToken)
        {
            user.Registration_Confirmed = confirmed;
            return Task.FromResult(0);
        }

        public async Task<User> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var users = await _entityCache.Users();
            return users.FirstOrDefault(u => u.Email_Address == normalizedEmail);
        }

        public Task<string> GetNormalizedEmailAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Email_Address);
        }

        public Task SetNormalizedEmailAsync(User user, string normalizedEmail, CancellationToken cancellationToken)
        {
            user.Email_Address = normalizedEmail;
            return Task.FromResult(0);
        }

        public async Task AddToRoleAsync(User user, string roleName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var roles = await _entityCache.Roles();
            var role = roles.FirstOrDefault(r => r.Name == roleName.ToUpper());

            if (role == null)
            {
                throw new Exception("Role does not exist");
            }

            if (!role.Is_Deleted)
            {
                throw new Exception("Cannot add user to a role that has been deactivated");
            }

            using (var uow = _uowFactory.GetUnitOfWork())
            {
                var userRoleId = await uow.UserRepo.CreateUserRole(new CreateUserRoleRequest()
                {
                    User_Id = user.Id,
                    Role_Id = role.Id,
                    Created_By = user.Created_By
                });

                uow.Commit();
            }
            _entityCache.Remove(CacheConstants.UserRoles);
        }

        public async Task RemoveFromRoleAsync(User user, string roleName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var roles = await _entityCache.Roles();
            var role = roles.FirstOrDefault(r => r.Name == roleName.ToUpper());

            if (role == null)
            {
                throw new Exception("Role does not exist");
            }

            using (var uow = _uowFactory.GetUnitOfWork())
            {
                await uow.UserRepo.DeleteUserRole(new DeleteUserRoleRequest()
                {
                    User_Id = user.Id,
                    Role_Id = role.Id,
                    Updated_By = user.Updated_By
                });

                uow.Commit();
            }
            _entityCache.Remove(CacheConstants.UserRoles);
        }

        public async Task<IList<string>> GetRolesAsync(User user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var roles = await _entityCache.Roles();
            var userRoles = await _entityCache.UserRoles();

            var roleIds = userRoles.Where(ur => ur.UserId == user.Id).Select(ur => ur.RoleId);
            return roles.Where(r => roleIds.Contains(r.Id)).Select(r => r.Name).ToList();
        }

        public async Task<bool> IsInRoleAsync(User user, string roleName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var roles = await _entityCache.Roles();
            var role = roles.FirstOrDefault(r => r.Name == roleName.ToUpper());

            if (role == null)
            {
                throw new Exception("Role does not exist");
            }

            var userRoles = await _entityCache.UserRoles();
            userRoles = userRoles.Where(ur => ur.Is_Deleted && ur.UserId == user.Id).ToList();

            return userRoles.Any(ur => ur.RoleId == role.Id);
        }

        public async Task<IList<User>> GetUsersInRoleAsync(string roleName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var roles = await _entityCache.Roles();
            var role = roles.FirstOrDefault(r => r.Name == roleName.ToUpper());

            if (role == null)
            {
                throw new Exception("Role does not exist");
            }

            var userRoles = await _entityCache.UserRoles();
            userRoles = userRoles.Where(ur => ur.RoleId == role.Id && ur.Is_Deleted).ToList();

            var userIds = userRoles.Select(ur => ur.UserId);
            var users = await _entityCache.Users();

            return users.Where(u => userIds.Contains(u.Id)).ToList();
        }

        public async Task SetPasswordHashAsync(User user, string passwordHash, CancellationToken cancellationToken)
        {
            user.Password_Hash = passwordHash;
        }

        public async Task<string> GetPasswordHashAsync(User user, CancellationToken cancellationToken)
        {
            return user.Password_Hash;
        }

        public async Task<bool> HasPasswordAsync(User user, CancellationToken cancellationToken)
        {
            return string.IsNullOrEmpty(user.Password_Hash);
        }

        public void Dispose()
        {
            // Nothing to dispose.
        }

        #endregion
    }
}