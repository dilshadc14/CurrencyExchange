using Microsoft.AspNetCore.Http;
using System.Diagnostics;
using System.Threading.Tasks;

namespace CurrencyExchange.MiddleWares
{
    
    public class ResponseTimeMiddleware
    {
        private readonly RequestDelegate _next;

        public ResponseTimeMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();
            context.Response.OnStarting(() =>
            {
                stopwatch.Stop();
                context.Items["ResponseTime"] = stopwatch.ElapsedMilliseconds;
                return Task.CompletedTask;
            });

            await _next(context);
        }
    }
}
