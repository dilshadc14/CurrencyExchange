using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CurrencyExchange.BusinessModels.Entities;

namespace CurrencyExchange.DataAccess.interfaces
{
    public interface IExchangeRateRepository
    {

        Task<ExchangeRateDTO> GetLatestRateAsync(string baseCurrency, string targetCurrency);
        Task<IEnumerable<ExchangeRate>> GetHistoricalRatesAsync(string baseCurrency, DateTime startDate, DateTime endDate);
    }
}
