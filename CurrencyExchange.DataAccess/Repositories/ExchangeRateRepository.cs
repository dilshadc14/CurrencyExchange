using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using CurrencyExchange.BusinessModels.Entities;
using CurrencyExchange.DataAccess.interfaces;

namespace CurrencyExchange.DataAccess.Repositories
{
    public class ExchangeRateRepository : IExchangeRateRepository
    {
        private readonly HttpClient _httpClient;
        private readonly string _frankfurterApiUrl = "https://api.frankfurter.app/latest";

        public ExchangeRateRepository(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<ExchangeRateDTO> GetLatestRateAsync(string baseCurrency, string targetCurrency)
        {       
            var response = await _httpClient.GetFromJsonAsync<ExchangeRateDTO>($"{_frankfurterApiUrl}?base={baseCurrency}");
            if (response == null)
            {
                throw new Exception("Failed to fetch exchange rates from the API.");
            }
            if (response !=null && !response.Rates.ContainsKey(targetCurrency))
            {
                throw new KeyNotFoundException($"Target currency '{targetCurrency}' not found in the response.");
            }

            return response;
        }

        public async Task<IEnumerable<ExchangeRate>> GetHistoricalRatesAsync(string baseCurrency, DateTime startDate, DateTime endDate)
        {

            var response = await _httpClient.GetStringAsync($"{_frankfurterApiUrl}?base={baseCurrency}");
            
            var data= JsonSerializer.Deserialize<dynamic>(response);
            //var response = await _httpClient.GetFromJsonAsync<IEnumerable<ExchangeRate>>($"https://api.frankfurter.app/{startDate:yyyy-MM-dd}..{endDate:yyyy-MM-dd}?base={baseCurrency}");
            return null;
        }
    }
}

