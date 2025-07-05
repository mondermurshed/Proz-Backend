// PollyPolicyRegistry.cs
using Polly;
using Polly.Retry;

public static class PollyPolicyRegistry
{
    public static AsyncRetryPolicy CreateDefaultRetryPolicy()
    {
        return Policy //This is Polly’s main tool to build retry rules.
              .Handle<Exception>() //  This says: "Retry only if any Exception happens" (any error, like internet not working, or SMTP server not found). You can also handle specific exceptions
            .WaitAndRetryAsync(  //WaitAndRetryAsync is a method that that will retry after a period of time pass (you can use a lot of methods but i think this is the best one. This method will take parameters up to 3 parameters.
                retryCount: 3,
                sleepDurationProvider: attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)), //This line tells Polly how long to wait before each retry. sleepDurationProvider: This is the name of a setting that Polly wants.Polly says: "Hey, tell me how long I should wait between retries." So, we're giving it a rule: sleepDurationProvider. attempt => ... is a This is a lambda expression, which is a short way of writing a function, it means "For each retry attempt, do something.". BTW attempt means we are in the retry number what ? like if this was our first time to retry then it will be 1, if it's second then it will be 2 etc.. so in short it means "When Polly gives me the retry attempt number, I’ll give it back a time to wait.".  Math.Pow(x, y) means: "x to the power of y". So Math.Pow(2, attempt) gives us: 1st attempt: 2¹ = 2 seconds, 2nd attempt: 2² = 4 seconds and finally 3rd attempt: 2³ = 8 seconds. So all this line means "For each retry attempt, wait 2^attempt seconds before trying again."
                onRetry: (exception, delay, attempt, context) => { ////This tells Polly what to do each time it retries. It's like saying "If I do a retry, this is like polly library tell you (the programmer) if i did retry executing something then what are the things i should take action for?" You answer: "Yes! I’ll give you a small function that runs each time you retry.". Now this "(exception, delay, attempt, context) =>" is a a lambda expression way to create a function and we are passing to this function four things, exception (The error that caused the retry), Delay (	How long Polly will wait before the next retry), attempt (	Which retry is this? (1, 2, or 3)) and the forth one (context) is not important for now. //Now inside the { ... } block, we can write code that runs when a retry happens (like the  Console.WriteLine) we did. You can also put log statement rather than console.writeLine() because log here is extra important + don't forget to pass the user's id or username so we can know to who the error occur.
                    Console.WriteLine($"Retry {attempt} after {delay} due to {exception.GetType().Name}"); 
                });
    }
}
