using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Template.Infrastructure.Identity
{
    public static class ApplicationPermissions
    {
        #region Fields

        public static ReadOnlyCollection<ApplicationPermission> AllPermissions;

        public const string UserPermissionGroupName = "User Permissions";
        public static ApplicationPermission LikeThings = new ApplicationPermission("Like Things", "things.like", UserPermissionGroupName, "Permission to like stores and products");

        public const string AdminPermissionGroupName = "Administrator Permissions";
        public static ApplicationPermission ViewUsers = new ApplicationPermission("View Users", "users.view", AdminPermissionGroupName, "Permission to view other users account details");
        public static ApplicationPermission ManageUsers = new ApplicationPermission("Manage Users", "users.manage", AdminPermissionGroupName, "Permission to create, delete and modify other users account details");
        public static ApplicationPermission ViewRoles = new ApplicationPermission("View Roles", "roles.view", AdminPermissionGroupName, "Permission to view available roles");
        public static ApplicationPermission ManageRoles = new ApplicationPermission("Manage Roles", "roles.manage", AdminPermissionGroupName, "Permission to create, delete and modify roles");
        public static ApplicationPermission ViewAdminDashboard = new ApplicationPermission("View Admin Dashboard", "admin.viewdashboard", AdminPermissionGroupName, "Permission to view available roles");
        public static ApplicationPermission ManageStores = new ApplicationPermission("Manage Stores", "stores.manage", AdminPermissionGroupName, "Permission to create, delete and modify stores");

        public const string StorePermissionGroupName = "Store Permissions";
        public static ApplicationPermission ManageStore = new ApplicationPermission("Manage Store", "store.manage", StorePermissionGroupName, "Permission to manage a store");
        public static ApplicationPermission ManageStoreProducts = new ApplicationPermission("Manage Store Products", "store.manageproducts", StorePermissionGroupName, "Permission to manage a store's products");

        #endregion

        #region Constructor

        static ApplicationPermissions()
        {
            List<ApplicationPermission> allPermissions = new List<ApplicationPermission>()
            {
                LikeThings,

                ViewUsers,
                ManageUsers,
                ViewRoles,
                ManageRoles,
                ViewAdminDashboard,
                ManageStores,

                ManageStore,
                ManageStoreProducts
            };

            AllPermissions = allPermissions.AsReadOnly();
        }

        #endregion

        #region Public Methods

        public static ApplicationPermission GetPermissionByName(string permissionName)
        {
            return AllPermissions.Where(p => p.Name == permissionName).FirstOrDefault();
        }

        public static ApplicationPermission GetPermissionByValue(string permissionValue)
        {
            return AllPermissions.Where(p => p.Value == permissionValue).FirstOrDefault();
        }

        public static string[] GetAllPermissionValues()
        {
            return AllPermissions.Select(p => p.Value).ToArray();
        }

        public static string[] GetAdministrativePermissionValues()
        {
            return new string[] { ManageUsers, ManageRoles, ManageStores, ViewAdminDashboard };
        }

        #endregion
    }

    public class ApplicationPermission
    {
        public ApplicationPermission()
        { }

        public ApplicationPermission(string name, string value, string groupName, string description = null)
        {
            Name = name;
            Value = value;
            GroupName = groupName;
            Description = description;
        }

        public string Name { get; set; }
        public string Value { get; set; }
        public string GroupName { get; set; }
        public string Description { get; set; }

        public override string ToString()
        {
            return Value;
        }

        public static implicit operator string(ApplicationPermission permission)
        {
            return permission.Value;
        }
    }
}
