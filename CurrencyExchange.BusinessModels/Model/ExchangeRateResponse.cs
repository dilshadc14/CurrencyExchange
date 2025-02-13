using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurrencyExchange.BusinessModels.Model
{
    public class ExchangeRateResponse:BaseModelResponse
    {
        public string BaseCurrency { get; set; }
        public DateTime Date { get; set; }
        public Dictionary<string, decimal> Rates { get; set; }
        
      //  public List<CurrencyRates> CurrencyRates { get; set; }
    }
}
