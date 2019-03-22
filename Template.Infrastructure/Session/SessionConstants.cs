namespace Template.Infrastructure.Session
{
    public class SessionConstants
    {
        public const string SessionEntity = "SessionEntity";
        public const string UserEntity = "UserEntity";

        public class Events
        {
            public const string Error = "ERROR";
            public const string UserSignedIn = "USER_SIGNED_IN";
            public const string UserUpdatedProfile = "USER_UPDATED_PROFILE";
        }
    }
}
