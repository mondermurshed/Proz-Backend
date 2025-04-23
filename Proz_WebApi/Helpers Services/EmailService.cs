
using DnsClient;
using EasyCaching.Core;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Distributed;
using MimeKit;
using Proz_WebApi.Helpers_Types;

namespace Proz_WebApi.Helpers_Services
{
    //Email Normalization
    public class EmailNormalizer : ILookupNormalizer
    {
        public string? NormalizeName(string name)
        {
            // Normalize the username by trimming and converting to lowercase
            return name?.Trim().ToLowerInvariant();
        }

        public string NormalizeEmail(string email)
        {
            email = email.Trim().ToLowerInvariant();
            var parts = email.Split('@'); //this means will split the whole email into two parts (of course if there is only one '@' symbole") based on the '@' in which there will  be the name part and the the domain part (e.g.  gmail.com )  BTW THE FIRST PART IS THE ONE THAT WILL  CONTAIN THE '@' symbole
            if (parts.Length != 2) return null; //if there was more then 2 parts (there is more then one '@' then return null

            var local = parts[0] //make it equal to the name part 
                .Replace(".", "")       // replace any dot with a nothing (removes them)
                .Split('+')[0];         // splits the name part again into two parts and take the first parth (because as we said before the @ will be in the first part not in the second, so we are here getting rid of the '@' by the safest way possible)

            return $"{local}@{parts[1]}";
        }
    }
    //Domain Verification
    public class DomainVerifier
    {
        private readonly LookupClient _dnsClient;

        public DomainVerifier() => _dnsClient = new LookupClient();

        /// <summary>
        /// Verifies domain has valid MX/A records
        /// </summary>
        /// <param name="email">Normalized email</param>
        public async Task<bool> HasValidDomainAsync(string email)
        {
            var domain = email.Split('@').LastOrDefault();
            if (string.IsNullOrEmpty(domain)) return false;

            var mxCheck = await _dnsClient.QueryAsync(domain, QueryType.MX);
            var aCheck = await _dnsClient.QueryAsync(domain, QueryType.A);

            return mxCheck.Answers.Any() || aCheck.Answers.Any();
        }
    }
    // Verification Email
    public class VerificationEmailSender
    {
        private readonly SmtpConfig _config;

        public VerificationEmailSender(SmtpConfig config)
            => _config = config;

        /// <summary>
        /// Sends 6-digit verification code to email
        /// </summary>
        /// <param name="email">Normalized email address</param>
        /// <param name="code">6-digit verification code</param>
        public async Task SendVerificationCodeAsync(string email, string code)
        {
            using var client = new SmtpClient();

            await client.ConnectAsync(
                _config.Server,
                _config.Port,
                SecureSocketOptions.StartTls
            );

            await client.AuthenticateAsync(
                _config.Username,
                _config.Password
            );

            var message = new MimeMessage();
            message.From.Add(MailboxAddress.Parse(_config.FromEmail));
            message.To.Add(MailboxAddress.Parse(email));
            message.Subject = "Verify Your Email";
            message.Body = new TextPart("plain")
            {
                Text = $"Your verification code: {code}"
            };

            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }
    }
    // 5. Code Verification
    // ======================


    public class VerificationCodeService
    {
        private readonly IEasyCachingProvider _cache;

        // Inject the named Redis provider directly
        public VerificationCodeService(IEasyCachingProviderFactory factory)
        {
            // Grab the provider registered as "redis1"
            _cache = factory.GetCachingProvider("redis1");
        }

        public async Task<string> GenerateAndStoreCodeAsync(string email)
        {
            var key = $"verification:{email}";
            var code = Random.Shared.Next(100000, 999999).ToString();

            // Store with 15-minute TTL
            _cache.Set(key, code, TimeSpan.FromMinutes(15));
            return code;
        }

        public async Task<bool> ValidateCodeAsync(string email, string userCode)
        {
            var key = $"verification:{email}";
            var cache = await _cache.GetAsync<string>(key);
            if (!cache.HasValue) return false;
            return cache.Value.Equals(userCode);
        }
    }


}
