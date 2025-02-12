using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CurrencyExchange.BusinessModels.Entities;

namespace CurrencyExchange.BusinessModels.Interfaces
{
    public interface ICurrencyConverterService
    {
        Task<ExchangeRate> GetLatestRateAsync(string baseCurrency, string targetCurrency);
        Task<decimal> ConvertCurrencyAsync(string from, string to, decimal amount);
        Task<IEnumerable<ExchangeRate>> GetHistoricalRatesAsync(string baseCurrency, DateTime startDate, DateTime endDate);
    }
}
