namespace Template.Models.EmailTemplates
{
    public interface IEmailTemplate
    {
        string Subject { get; }

        string GetHTMLContent();
    }
}
