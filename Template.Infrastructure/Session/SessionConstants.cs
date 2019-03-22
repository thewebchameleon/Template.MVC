namespace Template.Infrastructure.Session
{
    public class SessionConstants
    {
        public const string SessionEntity = "SessionEntity";
        public const string UserEntity = "UserEntity";
        public const string SessionLogId = "SessionLogId";

        public class Events
        {
            public const string Error = "ERROR";
            public const string UserLoggedIn = "USER_LOGGED_IN";
            public const string UserRegistered= "USER_REGISTERED";
            public const string UserUpdatedProfile = "USER_UPDATED_PROFILE";
        }
    }
}
