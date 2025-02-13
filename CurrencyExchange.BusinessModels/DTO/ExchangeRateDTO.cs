using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurrencyExchange.BusinessModels.DTO
{
    public class ExchangeRateDTO
    {
        public string BaseCurrency { get; set; }
        public Dictionary<string, decimal> Rates { get; set; }
        public DateTime Date { get; set; }
    }
}
