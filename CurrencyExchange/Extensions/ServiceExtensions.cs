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

