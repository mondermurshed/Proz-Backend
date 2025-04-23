namespace Proz_WebApi.Helpers_Types
{
    public class SmtpConfig
    {
        /// <summary> SMTP server host, e.g. "smtp.gmail.com" </summary>
        public string Server { get; set; }

        /// <summary> SMTP port, e.g. 587 for StartTLS or 465 for SSL </summary>
        public int Port { get; set; }

        /// <summary> Username for authenticating to the SMTP server </summary>
        public string Username { get; set; }

        /// <summary> Password or app‑specific password for the SMTP server </summary>
        public string Password { get; set; }

        /// <summary> “From” address that your app sends mail as </summary>
        public string FromEmail { get; set; }
    }
}
