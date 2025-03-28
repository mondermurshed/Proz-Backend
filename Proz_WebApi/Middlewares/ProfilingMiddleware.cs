//using System.Diagnostics;

//namespace Proz_WebApi.Middlewares
//{
//    public class ProfilingMiddleware
//    {
//        private readonly RequestDelegate _next;
//        private readonly ILogger<ProfilingMiddleware> _logger;

//        public ProfilingMiddleware(RequestDelegate next, ILogger<ProfilingMiddleware> logger)
//        {
//            _next = next;
//            _logger = logger;
//        }
//        public async Task Invoke (HttpContext context)
//        {
//            var timer = new Stopwatch();
//            timer.Start();
//            await _next(context);
//            timer.Stop();
//            _logger.LogInformation("Request {RequestPath} took {ElapsedMilliseconds} ms", context.Request.Path, timer.ElapsedMilliseconds);
//        }
//    }
//}
