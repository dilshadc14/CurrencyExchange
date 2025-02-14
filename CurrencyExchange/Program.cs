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
var builder = WebApplication.CreateBuilder(args);
string logFileName = $"logs/retry-log-{DateTime.UtcNow:yyyy-MM-dd}.txt";
builder.Services.AddControllers();
Log.Logger = new LoggerConfiguration()
    .WriteTo.File("logs/retry-log.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

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



// Add HttpClient with circuit breaker policy
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

builder.Services.AddAuthorization();


var app = builder.Build();

app.UseStaticFiles(); 
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