using CurrencyExchange.BusinessLayer.Interfaces;
using CurrencyExchange.BusinessLayer.Services;
using CurrencyExchange.DataAccess.interfaces;
using CurrencyExchange.DataAccess.Repositories;

namespace CurrencyExchange.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddSingleton<JwtService>();
            services.AddMemoryCache();
            services.AddScoped<IExchangeRateRepository, ExchangeRateRepository>();
            services.AddScoped<ICurrencyService, CurrencyService>();
            services.AddSingleton<ICurrencyValidationService, CurrencyValidationService>();
            return services;
        }
    }
}

//builder.Services.AddSingleton<ICurrencyValidationService, CurrencyValidationService>();
//builder.Host.UseSerilog();
//builder.Services.AddSingleton<JwtService>();
//builder.Services.AddMemoryCache();
//builder.Services.AddScoped<IExchangeRateRepository, ExchangeRateRepository>();
//builder.Services.AddScoped<ICurrencyService, CurrencyService>();
//builder.Services.Configure<CurrencySettings>(builder.Configuration.GetSection("CurrencySettings"));