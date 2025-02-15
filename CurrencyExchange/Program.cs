using System.Text;
using Asp.Versioning;

using CurrencyExchange.BusinessLayer.Interfaces;
using CurrencyExchange.BusinessLayer.Services;
using CurrencyExchange.BusinessModels.Interfaces;
using CurrencyExchange.DataAccess.interfaces;
using CurrencyExchange.DataAccess.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using CurrencyExchange;
using Microsoft.Extensions.FileProviders;
using Polly;
using Microsoft.Extensions.DependencyInjection;
using Polly.Extensions.Http;
using Serilog;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;
using CurrencyExchange.MiddleWares;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using OpenTelemetry.Logs;
using CurrencyExchange.Extensions;
var builder = WebApplication.CreateBuilder(args);
string logFileName = $"logs/retry-log-{DateTime.UtcNow:yyyy-MM-dd}.txt";
builder.Services.AddControllers();
//Log.Logger = new LoggerConfiguration()
//    .WriteTo.File("logs/retry-log.txt", rollingInterval: RollingInterval.Day)
//    .CreateLogger();

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .CreateLogger();




builder.Services.AddOpenTelemetry()
    .WithTracing(tracerProviderBuilder =>
    {
        tracerProviderBuilder
            .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("CurrencyExchange"))
            .AddAspNetCoreInstrumentation() 
            .AddConsoleExporter(); 
        //.AddOtlpExporter(options =>
        //    options.Endpoint = new Uri("http://localhost:4317") 
        //)
    });
builder.Logging.AddOpenTelemetry(options =>
{
    options.IncludeFormattedMessage = true;
   
});

builder.Host.UseSerilog();



var retryPolicy = HttpPolicyExtensions
    .HandleTransientHttpError() 
    .Or<TimeoutException>() 
    .OrResult(response => {
        return response.StatusCode == System.Net.HttpStatusCode.InternalServerError || // 500
                response.StatusCode == System.Net.HttpStatusCode.NotFound || // 404
                response.StatusCode == System.Net.HttpStatusCode.Forbidden
                || response.StatusCode == System.Net.HttpStatusCode.Unauthorized;
      })
    .WaitAndRetryAsync(
        retryCount: 3,
        sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
        onRetry: (result, delay, retryAttempt, context) =>
        {
            string logMessage = $"[{DateTime.UtcNow}] [Retry {retryAttempt}] Waiting {delay.TotalSeconds} sec due to {result.Exception?.Message ?? result.Result.StatusCode.ToString()}";
            Log.Information(logMessage);
         
        });
var circuitBreakerPolicy = HttpPolicyExtensions
    .HandleTransientHttpError()
    .Or<TimeoutException>()
    .OrResult(response => {
        return response.StatusCode == System.Net.HttpStatusCode.InternalServerError || // 500
                response.StatusCode == System.Net.HttpStatusCode.NotFound || // 404
                response.StatusCode == System.Net.HttpStatusCode.Forbidden
                || response.StatusCode == System.Net.HttpStatusCode.Unauthorized;
    })
    .CircuitBreakerAsync(
        handledEventsAllowedBeforeBreaking: 3, 
        durationOfBreak: TimeSpan.FromSeconds(30), 
        onBreak: (exception, duration) =>
        {
            string logMessage = $"Circuit opened. Blocking requests for {duration.TotalSeconds} seconds";
             Log.Information(logMessage);
        },
        onReset: () =>
        {
            string logMessage = $"Circuit reset (closed)";
             Log.Information(logMessage);
        });
builder.Services.AddHttpClient("ExternalApi", client =>
{
    client.BaseAddress = new Uri("https://api.frankfurter.app/");
}).AddPolicyHandler(retryPolicy)
.AddPolicyHandler(circuitBreakerPolicy);
builder.Services.AddSingleton<JwtService>();
builder.Services.AddMemoryCache();
builder.Services.AddScoped<IExchangeRateRepository, ExchangeRateRepository>();
builder.Services.AddScoped<ICurrencyService, CurrencyService>();
builder.Services.Configure<CurrencySettings>(builder.Configuration.GetSection("CurrencySettings"));
builder.Services.AddSingleton<ICurrencyValidationService, CurrencyValidationService>();
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
});
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "Exchange Rate API", Version = "v1" });
    options.MapType<DateTime>(() => new OpenApiSchema
    {
        Type = "string",
        Format = "date"
    });


    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        //Type = SecuritySchemeType.ApiKey,
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter your JWT token in the format: Bearer {token}"
    });

    // Make Swagger require a token for all endpoints
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
             new string[] {}
        }
    });
});
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = Encoding.ASCII.GetBytes(jwtSettings["Key"]);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(secretKey)
        };
    });

//builder.Services.AddAuthorization();
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    options.AddPolicy("UserOrAdmin", policy => policy.RequireRole("User", "Admin"));
});
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = 429; 
    options.OnRejected = (context, cancellationToken) =>
    {
        context.HttpContext.Response.WriteAsync("Rate limit exceeded. Please try again later.", cancellationToken);
        return new ValueTask();
    };

    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? "global",
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 10, 
                Window = TimeSpan.FromMinutes(1), 
                QueueLimit = 2
            }
        )
    );
    options.AddPolicy<string>("StrictLimit", context =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? "strict",
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 5, 
                Window = TimeSpan.FromMinutes(1), 
                QueueLimit = 0 
            }
        )
    );
    options.AddPolicy<Guid>("UserLimit", context =>
    {
        var userId = context.User.FindFirst("UserId")?.Value;
        return RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: userId != null ? Guid.Parse(userId) : Guid.NewGuid(),
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 5,
                Window = TimeSpan.FromMinutes(1)
            }
        );
    });
    options.AddPolicy("LowTrafficPolicy", context =>
       RateLimitPartition.GetFixedWindowLimiter(
           partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? "low-traffic",
           factory: _ => new FixedWindowRateLimiterOptions
           {
               PermitLimit = 10,
               Window = TimeSpan.FromMinutes(1),
               QueueLimit = 0 
           }
       )
   );
});



var app = builder.Build();
app.UseMiddleware<ResponseTimeMiddleware>();

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
        if (httpContext.Items.TryGetValue("ResponseTime", out var responseTime))
        {
            diagnosticContext.Set("ResponseTime", $"{responseTime}ms");
        }
    };
});
app.UseStaticFiles();
app.UseRateLimiter();
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(Directory.GetCurrentDirectory(), "js")),
    RequestPath = "/js"
});
app.UseMiddleware<ExceptionHandlingMiddleware>();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.InjectJavascript("/js/custom.js");
    });
}
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();