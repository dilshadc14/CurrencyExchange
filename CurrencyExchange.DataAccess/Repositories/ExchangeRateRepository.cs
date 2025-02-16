using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using CurrencyExchange.DataAccess.interfaces;
using Microsoft.Extensions.Caching.Memory;
using CurrencyExchange.BusinessModels.DTO;
using CurrencyExchange.BusinessModels.Entities;
using CurrencyExchange.BusinessModels.Model;
using Polly;
using Polly.CircuitBreaker;
using Polly.Extensions.Http;
using Polly.Retry;
using CurrencyExchange.BusinessModels;
using System.Net;

namespace CurrencyExchange.DataAccess.Repositories
{
    public class ExchangeRateRepository : IExchangeRateRepository
    {
        private readonly HttpClient _httpClient;
        private readonly string _frankfurterApiUrl = "https://api.frankfurter.app";
        private readonly IMemoryCache _cache;
       

        public ExchangeRateRepository(IMemoryCache cache,HttpClient httpClient)
        {
            _cache = cache;
            _httpClient = httpClient;
         
        }

        public async Task<ExchangeRateDTO> FetchLatestRatesAsync(string baseCurrency)
        {
            var cacheKey = $"LatestRates_{baseCurrency}";


            if (!_cache.TryGetValue(cacheKey, out ExchangeRateDTO rates))
            {
                //var response = await _httpClient.GetFromJsonAsync<ExchangeRateDTO>($"{_frankfurterApiUrl}/latest?from={baseCurrency}");
                //_cache.Set(cacheKey, response, TimeSpan.FromMinutes(10));
                //rates = response;
                var response = await _httpClient.GetAsync($"{_frankfurterApiUrl}/latest?from={baseCurrency}");
                if (!response.IsSuccessStatusCode)
                {
                    
                    switch (response.StatusCode)
                    {
                        case System.Net.HttpStatusCode.InternalServerError: 
                            throw new ApiException(HttpStatusCode.InternalServerError, "Internal server error occurred.");
                        case System.Net.HttpStatusCode.NotFound: 
                            throw new ApiException(HttpStatusCode.NotFound, "The Requested Currency Not  Found");
                        case System.Net.HttpStatusCode.Forbidden: 
                            throw new ApiException(HttpStatusCode.Forbidden, "Access forbidden.");
                        default:
                            throw new ApiException(HttpStatusCode.BadRequest, $"Failed to fetch exchange rates. Status code: {response.StatusCode}");
                    }
                }
                rates = await response.Content.ReadFromJsonAsync<ExchangeRateDTO>();
                _cache.Set(cacheKey, rates, TimeSpan.FromMinutes(10));
               

            }



            return rates;
          
        }

        public async Task<ExchangeRateDTO> GetLatestRateAsync(string baseCurrency, string targetCurrency)
        {       
            var response = await _httpClient.GetFromJsonAsync<ExchangeRateDTO>($"{_frankfurterApiUrl}/latest?base={baseCurrency}");
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

      

        public async Task<HistoricalExchangeRatesDTO> GetHistoricalRatesAsync(HistoricalRatesRequest request)
        {
            var cacheKey = $"HistoricalRates_{request.BaseCurrency}_{request.StartDate:yyyy-MM-dd}_{request.EndDate:yyyy-MM-dd}";
            if (!_cache.TryGetValue(cacheKey, out HistoricalExchangeRatesDTO rates))
            {
                var response = await _httpClient.GetFromJsonAsync<HistoricalExchangeRatesDTO>($"{_frankfurterApiUrl}/{request.StartDate:yyyy-MM-dd}..{request.EndDate:yyyy-MM-dd}?from={request.BaseCurrency}");
                rates = new HistoricalExchangeRatesDTO
                {
                    Base = request.BaseCurrency,
                    StartDate = request.StartDate,
                    EndDate = request.EndDate,
                    Rates = response.Rates

                };
                _cache.Set(cacheKey, rates, TimeSpan.FromMinutes(10));
            }
            return rates;
        }
    }


}


