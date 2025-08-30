using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Proz_WebApi.Helpers_Services.SignleR_Logic
{

    //public interface IChatClient
    //{
    //    Task ReceiveMessage(string user, string message);
    //}

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class MainHub : Hub/*<IChatClient>*/
    {
        // This hub is server-push only. No methods required for client calls.
        //public async Task SendMessage(string user, string message)
        //{
        //    Console.WriteLine($"Message from {user}: {message}");
        //    // Broadcast the message to all clients
        //    await Clients.All.ReceiveMessage(user, message);
        //}
    }
}
