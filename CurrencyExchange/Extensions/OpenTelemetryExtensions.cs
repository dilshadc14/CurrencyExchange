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


//builder.Services.AddOpenTelemetry()
//    .WithTracing(tracerProviderBuilder =>
//    {
//        tracerProviderBuilder
//            .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("CurrencyExchange"))
//            .AddAspNetCoreInstrumentation() 
//            .AddConsoleExporter(); 
//        //.AddOtlpExporter(options =>
//        //    options.Endpoint = new Uri("https://localhost:7279/") 
//        //)
//    });
//builder.Logging.AddOpenTelemetry(options =>
//{
//    options.IncludeFormattedMessage = true;

//});