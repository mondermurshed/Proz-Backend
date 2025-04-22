
using DnsClient;
using Microsoft.AspNetCore.Identity;

namespace Proz_WebApi.Helpers_Services
{
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
    public class EmailDomainValidator
    {
        public static async Task<bool> IsValidEmailDomainAsync(string domain)
        {
            var lookup = new LookupClient();
            var result = await lookup.QueryAsync(domain, QueryType.MX);
        
            if (!result.Answers.Any() && !(await lookup.QueryAsync(domain, QueryType.A)).Answers.Any())
            {
                return false;
            }
            return true;
        }
    }
    public class EmailSecurityLogic
    {
    public 
    }
}
