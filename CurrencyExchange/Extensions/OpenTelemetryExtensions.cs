using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace CurrencyExchange.Extensions
{
    public static class OpenTelemetryExtensions
    {
        public static WebApplicationBuilder AddOpenTelemetryTracing(this WebApplicationBuilder builder)
        {
            builder.Services.AddOpenTelemetry()
                .WithTracing(tracerProviderBuilder =>
                {
                    tracerProviderBuilder
                        .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("CurrencyExchange"))
                        .AddAspNetCoreInstrumentation()
                        .AddHttpClientInstrumentation()
                        .AddConsoleExporter();
                });

            builder.Logging.AddOpenTelemetry(options =>
            {
                options.IncludeFormattedMessage = true;
            });

            return builder;
        }
    }
}


