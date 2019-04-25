namespace Template.Models.EmailTemplates
{
    public class ForgotPasswordTemplate : IEmailTemplate
    {
        public string ResetPasswordURL { get; set; }

        public string Subject => "Forgot password request";

        public string GetHTMLContent()
        {
            return $@"
                <html>
                    <h2>A forgot password request has been made to your account</h2>
                    <h4>Please reset your password by clicking the link below</h4>
                    <a href='{ResetPasswordURL}'>Reset Password</a>
                </html>
            ";
        }
    }
}
