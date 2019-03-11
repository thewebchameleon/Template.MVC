namespace Template.Models
{
    public enum TokenTypeEnum : int
    {
        Undefined = 0,
        AccountActivation = 1
    }

    public enum DuplicateCheckProperty
    {
        Undefined = 0,
        EmailAddress = 1,
        MobileNumber = 2
    }
}
