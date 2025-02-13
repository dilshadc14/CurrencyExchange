using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CurrencyExchange.BusinessModels.DTO;

namespace CurrencyExchange.DataAccess.interfaces
{
    public interface IExchangeRateRepository
    {

        Task<ExchangeRateDTO> FetchLatestRatesAsync(string baseCurrency);
        Task<ExchangeRateDTO> GetLatestRateAsync(string baseCurrency, string targetCurrency);
        Task<IEnumerable<ExchangeRateDTO>> GetHistoricalRatesAsync(string baseCurrency, DateTime startDate, DateTime endDate);
    }
}
