
using System.Threading.Tasks;
using CurrencyExchange.BusinessModels.Entities;
using CurrencyExchange.BusinessModels.Model;

namespace CurrencyExchange.BusinessLayer.Interfaces
{
    public interface ICurrencyService
    {
        Task<CurrencyConversionResponse> ConvertCurrencyAsync(CurrencyConversionRequest request);
        //Task<CurrencyConversionResponse> FetchLatestRatesAsync(string baseCurrency);        
        Task<ExchangeRateResponse> FetchLatestRatesAsync(string baseCurrency);
      
        Task<HistoricalRatesResponse> GetHistoricalRatesAsync(HistoricalRatesRequest request);
    }
}
