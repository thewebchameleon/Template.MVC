namespace Template.Models.EmailTemplates
{
    public class AccountActivationTemplate : IEmailTemplate
    {
        public string ActivationURL { get; set; }

        public string Subject => "Please activate your account";

        public string GetHTMLContent()
        {
            return $@"
                <html>
                    <h2>Please activate your account by clicking the link below</h2>
                    <a href='{ActivationURL}'>Activate</a>
                </html>
            ";
        }
    }
}
