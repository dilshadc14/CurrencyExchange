using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using CurrencyExchange.BusinessLayer.Common;
using CurrencyExchange.BusinessLayer.Interfaces;
using CurrencyExchange.BusinessModels.Model;
using CurrencyExchange.DataAccess.interfaces;



namespace CurrencyExchange.BusinessLayer.Services
{
    public class CurrencyService : ICurrencyService
    {
        private readonly IExchangeRateRepository _exchangeRateRepository;

        public CurrencyService(IExchangeRateRepository exchangeRateRepository)
        {
            _exchangeRateRepository = exchangeRateRepository;
        }

        public async Task<CurrencyConversionResponse> ConvertCurrencyAsync(CurrencyConversionRequest request)
        {
            var response = new CurrencyConversionResponse();

            // Validate the request early
            if (string.IsNullOrEmpty(request.From) || string.IsNullOrEmpty(request.To))
            {
                response.Status = false;
                response.Message = Status.InvalidRequest.GetDescription();
                return response;
            }

            // Normalize the currency codes once
            string fromCurrency = request.From.Trim().ToUpper();
            string toCurrency = request.To.Trim().ToUpper();

            // Ensure the From and To currencies are different to avoid unnecessary conversion
            if (fromCurrency == toCurrency)
            {
                response.Status = false;
                response.Message =  "From and To currencies cannot be the same";
                return response;
            }

            try
            {
                // Fetch the latest exchange rate once and validate the result
                var rate = await _exchangeRateRepository.GetLatestRateAsync(fromCurrency, toCurrency);

                if (rate?.Rates == null || !rate.Rates.ContainsKey(toCurrency))
                {
                    response.Status = false;
                    response.Message = $"Exchange rate not found for {fromCurrency} to {toCurrency}.";
                    return response;
                }


                decimal conversionRate = rate.Rates[toCurrency];
                decimal convertedAmount = request.Amount * conversionRate;

                response.From = fromCurrency;
                response.To = toCurrency;
                response.Amount = request.Amount;
                response.Status = true;
                response.Message = Status.Success.ToString();
                response.ConvertedAmount = convertedAmount;
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Message = $"An error occurred: {ex.Message}";
            }

            return response;
        }
    }
}
