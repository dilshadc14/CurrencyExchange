using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurrencyExchange.BusinessModels.Entities
{
    public class ExchangeRate
    {
        public string BaseCurrency { get; set; }
        public string TargetCurrency { get; set; }
        public decimal Rate { get; set; }
        public DateTime Date { get; set; }
    }

    public class ExchangeRateDTO
    {
      
            public string Base { get; set; } 
            public DateTime Date { get; set; } 
            public Dictionary<string, decimal> Rates { get; set; } 
             public string Message { get; set; }
        
        
    }
}
