using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;

namespace CurrencyExchange.Extensions
{

    public static class LoggingExtensions
    {

        public static WebApplicationBuilder AddCustomLogging(this WebApplicationBuilder builder)
        {
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(builder.Configuration)
                .Enrich.FromLogContext()
                .CreateLogger();

            builder.Host.UseSerilog();
            return builder;
        }

        public static WebApplication UseRequestLogging(this WebApplication app)
        {
            app.UseSerilogRequestLogging(options =>
            {
                options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
                {
                    diagnosticContext.Set("ClientIP", httpContext.Connection.RemoteIpAddress?.ToString());
                    var clientId = httpContext.User.FindFirst("client_id")?.Value;
                    diagnosticContext.Set("ClientId", clientId);
                    diagnosticContext.Set("Method", httpContext.Request.Method);
                    diagnosticContext.Set("Endpoint", httpContext.Request.Path);
                    diagnosticContext.Set("ResponseCode", httpContext.Response.StatusCode);

                    if (httpContext.Items.TryGetValue("ResponseTime", out var rt))
                        diagnosticContext.Set("ResponseTime", $"{rt}ms");
                };
            });

            return app;
        }
    }
}
