using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Proz_WebApi.filters
{
    public class LogActivityFilter : IAsyncActionFilter
    {
        private readonly ILogger<LogActivityFilter> _logger;

        public LogActivityFilter(ILogger<LogActivityFilter> logger)
        {
            _logger= logger;
        }


        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            _logger.LogInformation("the user is trying to execute this method {visted-method}", context.ActionDescriptor.DisplayName);
           await next();
            _logger.LogInformation("and this is me after the user executed the method {visted-method}", context.ActionDescriptor.DisplayName);
        }
    }
}
