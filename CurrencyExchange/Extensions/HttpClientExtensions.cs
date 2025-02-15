
using Polly;
using Polly.Extensions.Http;
using Serilog;
namespace CurrencyExchange.Extensions
{
    public static class HttpClientExtensions
    {
        public static IServiceCollection AddResilientHttpClient(this IServiceCollection services)
        {
            var retryPolicy = HttpPolicyExtensions
                .HandleTransientHttpError()
                .Or<TimeoutException>()
                .OrResult(response => response.StatusCode is
                    System.Net.HttpStatusCode.InternalServerError or
                    System.Net.HttpStatusCode.NotFound or
                    System.Net.HttpStatusCode.Forbidden or
                    System.Net.HttpStatusCode.Unauthorized)
                .WaitAndRetryAsync(3, retryAttempt =>
                    TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    onRetry: (result, delay, retryAttempt, context) =>
                    {
                        Log.Information($"[Retry {retryAttempt}] Waiting {delay.TotalSeconds} sec");
                    });

            var circuitBreakerPolicy = HttpPolicyExtensions
                .HandleTransientHttpError()
                .OrResult(response => response.StatusCode is
                    System.Net.HttpStatusCode.InternalServerError or
                    System.Net.HttpStatusCode.NotFound or
                    System.Net.HttpStatusCode.Forbidden or
                    System.Net.HttpStatusCode.Unauthorized)
                .CircuitBreakerAsync(3, TimeSpan.FromSeconds(30),
                    (exception, duration) => Log.Information($"Circuit opened for {duration.TotalSeconds}s"),
                    () => Log.Information("Circuit reset"));

            services.AddHttpClient("ExternalApi", client =>
            {
                client.BaseAddress = new Uri("https://api.frankfurter.app/");
            })
                .AddPolicyHandler(retryPolicy)
                .AddPolicyHandler(circuitBreakerPolicy);

            return services;
        }
    }
}


