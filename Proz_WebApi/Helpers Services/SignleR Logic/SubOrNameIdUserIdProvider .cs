using Microsoft.AspNetCore.SignalR;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Proz_WebApi.Helpers_Services
{
    public class SubOrNameIdUserIdProvider : IUserIdProvider
    {
        public string? GetUserId(HubConnectionContext connection)
        {
            return connection.User?.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
         ?? connection.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }
    }
}

// this class "SubOrNameIdUserIdProvider" is actually very imporatnt, it inhert from IUserIdProvider interface which is works as the following: first we will need to know how the user can connect to signalR, first he pass his access token of JWT to here, if asp.net saw that he is authenticated (has a valid access token) then his connection will be saved, how? as the following: first the signalR will store two things, the connectionID (which is used to know where to send the message or the thing you want to send) and second is something called "UserIdentifier" now UserIdentifier is known by this class which is "SubOrNameIdUserIdProvider" !! 
//So it works as the following, when the user connected successfully the signalR must have a way to store a value to the "UserIdentifier" so it will be the id of that user. By default it will use "ClaimTypes.NameIdentifier" but we are here storing the user's id with our Sub claim, so we told it to store the Sub claim that it got it from the access token that was passed by the user and store it in the "UserIdentifier" place along with the connectionID. 
//So this class "SubOrNameIdUserIdProvider" is used in every time a user/client is connected to a hub of signalR, that's it!! because by default it will insert inside the "UserIdentifier" the NameIdentifier's value but we want the Sub claim value instead (because we store the id if the user inside the Sub claim). If