using Microsoft.AspNetCore.Identity;

namespace Proz_WebApi.Helpers_Types
{
    public class FinalResult
    {
        public FinalResult()
        {
           
            
        }

        public bool Succeeded { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; internal set; }
        public List<string> Errors { get; set; }
        public List<string> Messages { get; set; } // Wheb we see public string Message { get; init; } it means that the property can only be set during object initialization. After the object is created, the property becomes immutable (we can't change it! wh
        public int SuccessCount { get; set; }
        public int FailedCount { get; set; }

        public void AuthSuccess(string token, string refreshtoken)  //this is used for all successful opeartion that doesn't need a message, like use it when you want to return sucess and how want your controller (not service) to handel the success message to the user
        {
            Succeeded = true;
            AccessToken = token;
            RefreshToken = refreshtoken;
        }
    
    
  
   
    }
}
