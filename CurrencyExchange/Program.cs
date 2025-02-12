using Asp.Versioning;
 
using CurrencyExchange.BusinessLayer.Interfaces;
using CurrencyExchange.BusinessLayer.Services;
using CurrencyExchange.BusinessModels.Interfaces;
using CurrencyExchange.DataAccess.interfaces;
using CurrencyExchange.DataAccess.Repositories;
using Microsoft.OpenApi.Models;
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddHttpClient<IExchangeRateRepository, ExchangeRateRepository>();
builder.Services.AddScoped<ICurrencyService, CurrencyService>();

builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
});
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "Currency Converter API", Version = "v1" });
});

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();