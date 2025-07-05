using Microsoft.AspNetCore.Identity;

namespace Proz_WebApi.Helpers_Types
{
    public class FinalResult //i was wrong, you don't have to make only one type to handle all the final result like i am doing here with the "FinalResult" type, it returns all the result from any endpoint and this is wrong. You will need to make a new type if method fucntion differ (like not for every single method but for methods that really differ) 
    {
        public FinalResult()
        {
            Errors = new List<string>();
            Messages = new List<string>();
        }

        public bool Succeeded { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; internal set; }
        public List<string> Errors { get; set; }
        public List<string> Messages { get; set; } // Wheb we see public string Message { get; init; } it means that the property can only be set during object initialization. After the object is created, the property becomes immutable (we can't change it! wh
     
        public void AuthSuccess(string token, string refreshtoken)  //this is used for all successful opeartion that doesn't need a message, like use it when you want to return sucess and how want your controller (not service) to handel the success message to the user
        {
            Succeeded = true;
            AccessToken = token;
            RefreshToken = refreshtoken;

        }
    
    
  
   
    }
    public class FinalResultBulkOperations //i was wrong, you don't have to make only one type to handle all the final result like i am doing here with the "FinalResult" type, it returns all the result from any endpoint and this is wrong. You will need to make a new type if method fucntion differ (like not for every single method but for methods that really differ) 
    {
        public FinalResultBulkOperations()
        {
            Errors = new List<string>();
            Messages = new List<string>();
        }

        public bool Succeeded { get; set; }
        public List<string> Errors { get; set; }
        public List<string> Messages { get; set; } // Wheb we see public string Message { get; init; } it means that the property can only be set during object initialization. After the object is created, the property becomes immutable (we can't change it! wh
        public int SuccessCount { get; set; }
        public int FailedCount { get; set; }
        public int SkippedCount { get; set; }


    }
    public class FinalResultWithPasswordCheckingInfo
    {
        public FinalResultWithPasswordCheckingInfo()
        {
            Errors = new List<string>();
            Messages = new List<string>();
            Suggestions = new List<string>();
        }
        public bool Succeeded { get; set; }
        public List<string> Errors { get; set; }
        public List<string> Messages { get; set; } // Wheb we see public string Message { get; init; } it means that the property can only be set during object initialization. After the object is created, the property becomes immutable (we can't change it! wh
        public bool NewPasswordCause { get; set; }
        public int Score {  get; set; }
        public string Strength {  get; set; }
        public string CrackTime {  get; set; }
        public List<string> Suggestions {  get; set; }
    }
}
