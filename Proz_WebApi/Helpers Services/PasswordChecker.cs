using System.Text;
using Zxcvbn;

public static class PasswordChecker
{
    private static readonly HashSet<string> BannedPasswords = LoadBannedPasswords(); //this object will hold all the bad words , Hashset is like a list but better when holding a big number of items or words.
    private const int MinimumScore = 3;

    private static HashSet<string> LoadBannedPasswords() //this method resposible to locate the path (the file) and then split the file's content into a valid data and then store the BannedPasswords with these data
    {
        var filePath = Path.Combine(AppContext.BaseDirectory, "ListOfTheBadWordsFiles/en");
        var bytes = File.ReadAllBytes(filePath);
        var content = Encoding.UTF8.GetString(bytes);

        return new HashSet<string>(
            content.Split(new[] { '\n', '\r', '\0', '|' },
                          StringSplitOptions.RemoveEmptyEntries),
            StringComparer.OrdinalIgnoreCase
        );
    }

    public static bool IsPasswordBanned(string password) //here when we decide if the user's password is bad/banned word or not.
    { //we have two checks here! 
        return BannedPasswords.Contains(password) || //the first is Exact Match: Contains(password) if the user entered shit then this level will noticed this but it will not noticed if the user entered shit123 because it locates only the exact bad word.
               BannedPasswords.Any(banned =>
                   password.Contains(banned, StringComparison.OrdinalIgnoreCase)); //Here we have Partial Match check: Any(banned => password.Contains(banned))  which mean that this check can know even if user types hellofuckmy123.
    }

    public static PasswordStrengthResponse ValidatePassword(
     string password,
     string email,
     string username)
    {
        var response = new PasswordStrengthResponse
        {
            Suggestions = new List<string>()
        };

        if (IsPasswordBanned(password))
        {
            return new PasswordStrengthResponse
            {
                IsValid = false,
                Message = "Password contains banned words",
                Suggestions = new[] { "Try to clear your password from any bad words" }
            };
        }

        var userInputs = new[] { email, username };
        var result = Core.EvaluatePassword(password, userInputs);
        // result.CalcTime This is the time that this library took to know if it's weak or good. this is not measuring the password strength itself! but the efficency of the algorithm that is used by this library.
        //result.CrackTime  This is the estimated number of seconds required to crack the password under a specific attack scenario
        //result.CrackTimeDisplay same as the previous one but provide more human friendly form like "6 minutes", or "centuries" or "months" 
      
        response.Score = result.Score;
        response.Strength = GetStrengthLevel(result.Score);
        response.CrackTime = result.CrackTimeDisplay.OnlineThrottling100PerHour;
        response.Suggestions = result.Feedback.Suggestions;
        response.IsValid = result.Score >= MinimumScore;

        // Explicit if/else for message
        if (response.IsValid)
        {
            response.Message = "Password is valid!";
        }
        else
        {
            response.Message = "Password needs improvement";
        }

        return response;


    }
    private static string GetStrengthLevel(int score)
    {
        if (score == 0)
        {
            return "Very Weak";
        }
        else if (score == 1)
        {
            return "Weak";
        }
        else if (score == 2)
        {
            return "Not that good";
        }
        else if (score == 3)
        {
            return "Good";
        }
        else if (score == 4)
        {
            return "Excellent";
        }
        else
        {
            return "Unknown";
        }
    }
}

public class PasswordStrengthResponse //this is the second class!
{
    public int Score { get; set; } // 0-4
    public string Strength { get; set; }
    public string CrackTime { get; set; }
    public IEnumerable<string> Suggestions { get; set; }
    public bool IsValid { get; set; }
    public string Message { get; set; }
}