using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CurrencyExchange.BusinessLayer.Interfaces;
using CurrencyExchange.BusinessModels.Model;
using CurrencyExchange.DataAccess.interfaces;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
            var rate = await _exchangeRateRepository.GetLatestRateAsync(request.From, request.To);
            //  var convertedAmount = request.Amount * rate.Rate;
            decimal result = rate.Rates[request.To];
          
            //decimal usdRate = rate.Rates["USD"];
            var convertedAmount = request.Amount * result;
            return new CurrencyConversionResponse
            {
                From = request.From,
                To = request.To,
                Amount = request.Amount,
                ConvertedAmount = convertedAmount
            };
        }
    }
}
