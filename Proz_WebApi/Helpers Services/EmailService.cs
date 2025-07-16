
using Amazon.SimpleEmailV2.Model;
using Amazon.SimpleEmailV2;
using DnsClient;
using EasyCaching.Core;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MimeKit;
using Polly;
using Polly.Retry;
using Proz_WebApi.Helpers_Types;
using Proz_WebApi.Models.DesktopModels.Dto.Auth;
using System.Data.SqlTypes;

namespace Proz_WebApi.Helpers_Services
{
    //Email Normalization
    public class EmailNormalizer : ILookupNormalizer
    {
        public string NormalizeName(string name)
        {
            // Normalize the username by trimming and converting to lowercase
            return name.Trim().ToLowerInvariant();
        }

        public string? NormalizeEmail(string email)
        {
            //imagine the user entered this email "  My.Email+anything@gmail.com  " first we will trim it to remove all the spaces so it will be like this "My.Email+anything@gmail.com". Then we convert all the letters to lowercase letters so it will look like this "my.email+anything@gmail.com" then we will split and change so the final result will be "myemail@gmail.com"
            email = email.Trim().ToLowerInvariant();
            var parts = email.Split('@'); //this means will split the whole email into two parts (of course if there is only one '@' symbole") based on the '@' in which there will  be the name part and the the domain part (e.g.  gmail.com )  BTW THE FIRST PART IS THE ONE THAT WILL  CONTAIN THE '@' symbole
            if (parts.Length != 2) return null; //if there was more then 2 parts (there is more then one '@' then return null

            var local = parts[0] //make it equal to the name part 
                .Replace(".", "")       // replace any dot with a nothing (removes them)
                .Split('+')[0];         // splits the name part again into two parts and take the first parth (because as we said before the @ will be in the first part not in the second, so we are here getting rid of the '@' by the safest way possible). This .Split('+')[0]: will removes anything after a plus sign so for example if the user wrote "myEmail+anything@gmail.com" then we will get rid of anything after the + sign, and it will be "myEmail@gmail.com"


            return $"{local}@{parts[1]}";
        }
    }
    //Domain Verification
    public class DomainVerifier
    {
        private readonly LookupClient _dnsClient;
        private readonly AsyncRetryPolicy _retryPolicy;

        public DomainVerifier(LookupClient dnsClient, AsyncRetryPolicy retryPolicy)
        {
        _dnsClient = dnsClient;
        _retryPolicy = retryPolicy;
        }
      


        public async Task<bool> HasValidDomainAsync(string email) //This checks if the domain(like gmail.com) actually exists and can receive emails.
        {
            if (string.IsNullOrWhiteSpace(email) || !email.Contains('@'))
                return false;
            var domain = email.Split('@').Last().Trim(); //Takes everything after the @ — that’s the domain (like yahoo.com). Another information for you is that domain itself (the thing after the @ is called domain like gmail.com) like gmail.com is a domain but this domain actually contained of two parts The Name (Second-Level Domain - SLD) → Like google, facebook, gmail and the Extension (Top-Level Domain - TLD) → Like .com, .org, .net. Our method checks if the whole thing like SLD+TLD actually exisits and it not checking only one like you are thinking( like only SLD or only TLD )
            if (string.IsNullOrEmpty(domain)) return false;
            try
            {

            
            var MXresult = await _retryPolicy.ExecuteAsync(async () =>
            {
                return await _dnsClient.QueryAsync(domain, QueryType.MX);  //MX type means Asking: “Does this domain accept mail?” (MX = Mail eXchange) and there is another one which is A type that askes Asks: “Does this domain even exist and have a basic address?” (A = Address) but it's not recomended for an "email checking purposes" 
           
            });
            if (MXresult.Answers.MxRecords().Any())
            {
                return true;
            }
            else
            {
                var Aresult = await _retryPolicy.ExecuteAsync(async () =>
                {
                     return await _dnsClient.QueryAsync(domain, QueryType.A);
                });
                if (Aresult.Answers.ARecords().Any())
                { return true; }
                return false;
            }
            }
            catch
            {
                return false;
            }

        }
    }
    // Verification Email
    //public class VerificationEmailSender
    //{
    //    private readonly SmtpConfig _config;
    //    private readonly AsyncRetryPolicy _retryPolicy;

    //    public VerificationEmailSender(IOptions<SmtpConfig> options, AsyncRetryPolicy retryPolicy)
    //    {
    //        _config = options.Value;
    //        _retryPolicy = retryPolicy;
    //    }


    //    public async Task SendVerificationCodeAsync(string email, string code)
    //    {
    //        await _retryPolicy.ExecuteAsync(async () => //they are four operations in total (connect, auth, send, disconnect) this is why i putted the polly method in the beginning of this method because when retrying we need to execute these four operation all at once otherwise it will fail sending an mail to the user.
    //        {
    //            using var client = new SmtpClient();

    //        await client.ConnectAsync(
    //            _config.Server,
    //            _config.Port,
    //            SecureSocketOptions.StartTls
    //        );

    //        await client.AuthenticateAsync(
    //            _config.Username,
    //            _config.Password
    //        );

    //        var message = new MimeMessage(); //This line creates a new empty email message.
    //        message.From.Add(MailboxAddress.Parse(_config.FromEmail)); //This says who is sending the email.
    //        message.To.Add(MailboxAddress.Parse(email)); //This says who should receive the email (the user).
    //        message.Subject = "Verify Your Email"; //This is the subject line the user sees in their inbox.
    //        message.Body = new TextPart("plain") //This sets the body of the email (the message itself). "plain" means plain text, not HTML. 
    //        {
    //            Text = $"Your verification code: {code}" //and The content is: "Your verification code: 123456"
    //        };
           
    //            await client.SendAsync(message); //This sends the message to the SMTP server. and The server will forward it to the user.
    //            await client.DisconnectAsync(true); //This safely closes the connection to the mail server.
    //        });
           
    //    }
    //}


public class SesEmailSender
    {
        private readonly IAmazonSimpleEmailServiceV2 _sesClient;
        private readonly AsyncRetryPolicy _retryPolicy;
        private readonly string _fromEmail;

        public SesEmailSender(
            IAmazonSimpleEmailServiceV2 sesClient,
            AsyncRetryPolicy retryPolicy)
        {
            _sesClient = sesClient; // Store the AWS SES client in a variable so we can use it later.
            _retryPolicy = retryPolicy; // Store the retry logic so we can use it in case there's a problem.
            _fromEmail = Environment.GetEnvironmentVariable("SES_FROM_EMAIL"); // Get the verified email from environment variables.
        }

        public async Task<bool> SendVerificationCodeAsync(string toEmail, string code)
        {
            try
            {

           
            // Try sending the email. If something goes wrong (like internet issue), Polly will retry 3 times.
            await _retryPolicy.ExecuteAsync(async () =>
            {
                // Create the email request. This is like writing a letter.
                var request = new SendEmailRequest
                {
                    FromEmailAddress = _fromEmail, // This is who the email is from (must be verified in SES).
                    
                    Destination = new Destination
                    {
                        ToAddresses = new List<string> { toEmail } // Who the email will go to (the user).
                        
                    },

                    Content = new EmailContent
                    {
                        Simple = new Message
                        {
                            Subject = new Content
                            {
                                Data = "Verify Your Email" // What the user sees as the email subject.
                            },
                            Body = new Body
                            {
                                Html = new Content
                                {
                                      Data = $@"
                                       <html>
                      <body style='margin: 0; padding: 0; font-family: Arial, sans-serif; background-color: #2c4e20;'>
                  
                    <!-- Wrapper table -->
                    <table width='100%' cellpadding='0' cellspacing='0' style='background-color: #000000;'>
                     <tr>
                         <td align='center'>
                             <!-- Inner email container -->
                             <table width='100%' cellpadding='0' cellspacing='20' style='max-width: 600px; background-color: #1a1a1a; border-radius: 12px; overflow: hidden;'>
                
                       <!-- Large Logo Banner -->
                       <tr>
                           <td style='padding: 0;'>
                               <img src='https://img.prozsupport.xyz/LogoProz.jpg' alt='Proz Logo' style='width: 100%; display: block;' />
                           </td>
                       </tr>
   
                     <!-- Reset Password Heading -->
                     <tr>
                         <td align='center' style='padding: 25px 20px 20px 20px;'>
                             <h1 style='color: #ffffff; font-size: 24px; margin: 0;'>Verify Your Email</h1>
                         </td>
                     </tr>

                     <!-- Subtext -->
                     <tr>
                         <td align='center' style='padding: 0 30px 10px 30px;'>
                             <p style='color: #4d9759; font-size: 17px; margin: 0; line-height: 1.5;'>
                                 received a request to verify your email for the <strong>Proz Desktop Application</strong>.
                                
                             </p>
                         </td>
                     </tr>
                  <tr>
                         <td align='center' style='padding: 0 30px 40px 30px;'>
                             <p style='color: #4d9759; font-size: 17px; margin: 0; line-height: 1.5;'>
                               Here is your verification code : <span style='color: hsl(0, 0%, 100%);'>{code}</span> 
                             </p>
                         </td>
                     </tr>
                   
          
                                          <tr>
                           <td align='center' style='padding: 0;'>
                               <p style='color: #9cafc2; font-size: 14px; margin: 0;'>Thank you {toEmail}</p>
                               <p style='color: #9cafc2; font-size: 14px; margin: 0;'>Proz team.</p>
                           </td>
                       </tr>
                       
                                       
                       
                       
                       
                                            <!-- Footer -->
                                            <tr>
                                                <td align='center' style='padding: 70px 30px 30px 30px;'>
                                                    <p style='color: #dc8686; font-size: 14px; margin: 0;'>
                                                        If you didn't request this email, you can safely ignore it.
                                                   
                                                </td>
                                                
                                            </tr>
                       
                                        </table>
                                    </td>
                                </tr>
                            </table>
                       
                        </body>
                        </html>"
                                },
                                Text = new Content
                                {
                                    Data = $"Here is your verification code : {code}\n\nIf you didn't request this, please ignore the message."
                                }
                            }
                        }
                    }
                };

                // Send the email using AWS SES.
                await _sesClient.SendEmailAsync(request);
                 
            });
            }
            catch (Exception ex) //polly works as following if "_sesClient.SendEmailAsync(request);" returned exception error then polly will handle this exception secretly and will not tell the try-catch statements about it except if it retried (the amount you told it to retry) and still "_sesClient.SendEmailAsync(request);" giving error then polly will throw the exception and by doing this the catch statement will here this and will return false.
            {
                return false;
            }
            return true;
        }

        public async Task<bool> SendResetLinkAsync(string toEmail, string resetlink)
        {
          try
            {

           

            // Try sending the email. If something goes wrong (like internet issue), Polly will retry 3 times.
            await _retryPolicy.ExecuteAsync(async () =>
            {
                // Create the email request. This is like writing a letter.
                var request = new SendEmailRequest
                {
                    FromEmailAddress = _fromEmail, // This is who the email is from (must be verified in SES).

                    Destination = new Destination
                    {
                        ToAddresses = new List<string> { toEmail } // Who the email will go to (the user).

                    },

                    Content = new EmailContent
                    {
                        Simple = new Message
                        {
                            Subject = new Content
                            {
                                Data = "Reset you password!" // What the user sees as the email subject.
                            },
                            Body = new Body
                            {
                                Html = new Content
                                {
                                    Data = $@"
   <html>
 <body style='margin: 0; padding: 0; font-family: Arial, sans-serif; background-color: #2c4e20;'>

     <!-- Wrapper table -->
     <table width='100%' cellpadding='0' cellspacing='0' style='background-color: #000000;'>
         <tr>
             <td align='center'>
                 <!-- Inner email container -->
                 <table width='100%' cellpadding='0' cellspacing='20' style='max-width: 600px; background-color: #1a1a1a; border-radius: 12px; overflow: hidden;'>

                     <!-- Large Logo Banner -->
                     <tr>
                         <td style='padding: 0;'>
                             <img src='https://img.prozsupport.xyz/LogoProz.jpg' alt='Proz Logo' style='width: 100%; display: block;' />
                         </td>
                     </tr>

                     <!-- Reset Password Heading -->
                     <tr>
                         <td align='center' style='padding: 25px 20px 20px 20px;'>
                             <h1 style='color: #ffffff; font-size: 24px; margin: 0;'>Reset Your Password</h1>
                         </td>
                     </tr>

                     <!-- Subtext -->
                     <tr>
                         <td align='center' style='padding: 0 30px 20px 30px;'>
                             <p style='color: #4d9759; font-size: 17px; margin: 0; line-height: 1.5;'>
                                 We received a request to reset your password for the <strong>Proz Desktop Application</strong>.
                                 Click the button below to reset it.
                             </p>
                         </td>
                     </tr>

                     <!-- Reset Button -->
                     <tr>
                         <td align='center' style='padding: 5px 30px 30px 30px;'>
                             <a href='{resetlink}' style='
                                 background-color: #2c6e36;
                                 color: #ffffff;
                                 padding: 14px 28px;
                                 text-decoration: none;
                                 border-radius: 6px;
                                 font-weight: bold;
                                 font-size: 16px;
                                 display: inline-block;
                             '>
                                 Reset Password
                             </a>
                         </td>
                         
                     </tr>
                   <tr>
    <td align='center' style='padding: 0;'>
        <p style='color: #9cafc2; font-size: 14px; margin: 0;'>Thank you {toEmail}</p>
        <p style='color: #9cafc2; font-size: 14px; margin: 0;'>Proz team.</p>
    </td>
</tr>

                



                     <!-- Footer -->
                     <tr>
                         <td align='center' style='padding: 70px 30px 30px 30px;'>
                             <p style='color: #dc8686; font-size: 14px; margin: 0;'>
                                 If you didn't request this email, you can safely ignore it.
                            
                         </td>
                         
                     </tr>

                 </table>
             </td>
         </tr>
     </table>

 </body>
 </html>"
                                }




,

                                Text = new Content
                                {
                                    Data = $"{resetlink}\n\nIf you didn't request this, please ignore the message."
                                }
                            }
                        }
                    }
                };

                // Send the email using AWS SES.
                await _sesClient.SendEmailAsync(request);
                
            });
            }
            catch(Exception er) //polly works as following if "_sesClient.SendEmailAsync(request);" returned exception error then polly will handle this exception secretly and will not tell the try-catch statements about it except if it retried (the amount you told it to retry) and still "_sesClient.SendEmailAsync(request);" giving error then polly will throw the exception and by doing this the catch statement will here this and will return false.
            {
                return false;
            }
            return true;
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
            var value = Random.Shared.Next(100000, 999999).ToString(); //here it must be string because redis excepts only string values and not pure int, so this is why the code variable will be string

            
            await _cache.SetAsync(key, value, TimeSpan.FromSeconds(90));
            return value;
        }

        public async Task<bool> ValidateCodeAsync(string email, string userCode)
        {
            var key = $"verification:{email}";
            var value = await _cache.GetAsync<string>(key);
            if (!value.HasValue) return false;
            return value.Value.Equals(userCode);
        }
        public async Task StoreUserRegistrationDataTemp(UserRegisterationTemp userdata, string email)
        {
            var key = $"register:{email}";
            var value = userdata;
           await _cache.SetAsync(key , value, TimeSpan.FromSeconds(90));
        }
        public async Task<UserRegisterationTemp?> GetUserRegistrationDataTemp(string email)
        {
            var key = $"register:{email}";
            var value = await _cache.GetAsync<UserRegisterationTemp>(key);
            if (value.HasValue)
                return value.Value;
            else
            return null;
            
        }

        public async Task<bool> IsInCooldownAsync(string email)
        {
            var key = $"cooldown:{email}";
            var result = await _cache.GetAsync<string>(key);
           
            return result.HasValue;
        }

        public async Task StartCooldownAsync(string email)
        {
            var key = $"cooldown:{email}";
            var exists = await _cache.ExistsAsync(key);

            if (!exists)
            {
                await _cache.SetAsync(key, "cooling", TimeSpan.FromSeconds(5));
            }
        }

    }


}
