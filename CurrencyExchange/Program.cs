
using Asp.Versioning;
using Microsoft.Extensions.FileProviders;

using CurrencyExchange.MiddleWares;

using CurrencyExchange.Extensions;
using CurrencyExchange;
var builder = WebApplication.CreateBuilder(args);
//Configuration
builder.Services.AddControllers();

builder.Services
    .AddApplicationServices()
    .AddCustomSwagger()
    .AddJwtAuthentication(builder.Configuration)
    .AddResilientHttpClient()
    .AddCustomRateLimiting();

builder
    .AddCustomLogging()
    .AddOpenTelemetryTracing();

builder.Services.Configure<CurrencySettings>(builder.Configuration.GetSection("CurrencySettings"));
builder.Services.Configure<ClientIpSettings>(builder.Configuration.GetSection("ClientIpSettings"));
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
});


var app = builder.Build();
//middle ware
app.UseMiddleware<ResponseTimeMiddleware>();
app
   .UseRequestLogging()
   .UseStaticFiles()
   .UseRateLimiter()
   .UseMiddleware<ExceptionHandlingMiddleware>();



app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(Directory.GetCurrentDirectory(), "js")),
    RequestPath = "/js"
});

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
app.UseMiddleware<ClientIpMiddleware>();
app.Run();