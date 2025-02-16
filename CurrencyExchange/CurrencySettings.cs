using Microsoft.Extensions.Options;

namespace CurrencyExchange
{
    public class CurrencySettings
    {
        private HashSet<string> _excludedCurrencies;

        public string ExcludedCurrencies { get; set; } 

        public HashSet<string> GetExcludedCurrencies()
        {
            if (_excludedCurrencies == null)
            {
              
                _excludedCurrencies = ExcludedCurrencies?
                    .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(c => c.Trim().ToUpperInvariant())
                    .ToHashSet();
            }

            return _excludedCurrencies;
        }
    }

    public interface ICurrencyValidationService
    {
        bool IsCurrencyAllowed(string currencyCode);
    }

    public class CurrencyValidationService : ICurrencyValidationService
    {
        private readonly HashSet<string> _excludedCurrencies;

        public CurrencyValidationService(IOptions<CurrencySettings> currencySettings)
        {         
            _excludedCurrencies = currencySettings.Value.GetExcludedCurrencies();
        }
        public bool IsCurrencyAllowed(string currencyCode)
        {           
            return !_excludedCurrencies.Contains(currencyCode.ToUpperInvariant());
        }
    }

    public class ClientIpSettings
    {
        public string DefaultIp { get; set; }
    }
}
