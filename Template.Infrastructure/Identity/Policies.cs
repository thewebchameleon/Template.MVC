namespace Template.Infrastructure.Identity
{
    public class Policies
    {
        ///<summary>Policy to allow viewing all user records.</summary>
        public const string ViewAllUsersPolicy = "View All Users";

        ///<summary>Policy to allow adding, removing and updating all user records.</summary>
        public const string ManageAllUsersPolicy = "Manage All Users";

        /// <summary>Policy to allow viewing details of all roles.</summary>
        public const string ViewAllRolesPolicy = "View All Roles";

        /// <summary>Policy to allow adding, removing and updating all roles.</summary>
        public const string ManageAllRolesPolicy = "Manage All Roles";

        ///<summary>Policy to allow adding, removing and updating all store products.</summary>
        public const string ViewAdminDashboardPolicy = "View Admin Dashboard";

        ///<summary>Policy to allow adding, removing and updating all stores.</summary>
        public const string ManageStoresPolicy = "Manage Stores";



        ///<summary>Policy to allow adding, removing and updating a store.</summary>
        public const string ManageStorePolicy = "Manage Store";

        ///<summary>Policy to allow adding, removing and updating all store products.</summary>
        public const string ManageStoreProductsPolicy = "Manage Store Products";



        ///<summary>Policy to allow liking of products or stores.</summary>
        public const string LikeThings = "Like Things";
    }
}
