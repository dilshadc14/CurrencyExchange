using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CurrencyExchange.BusinessModels.Entities;
using CurrencyExchange.BusinessModels.Model;

namespace CurrencyExchange.Helper
{
    public static class DictionaryConverter
    {
        public static List<CurrencyRates> ConvertToExchangeRateList(Dictionary<DateTime, Dictionary<string, decimal>> rates)
        {
            return rates
                .SelectMany(dateRate => dateRate.Value
                    .Select(currencyRate => new CurrencyRates
                    {
                        ExchangeDate = dateRate.Key,
                        Currency = currencyRate.Key,
                        Rate = currencyRate.Value
                    }))
                .ToList();
        }
    }
    
}
