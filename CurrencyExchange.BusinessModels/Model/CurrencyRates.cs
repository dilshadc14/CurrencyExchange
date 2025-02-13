using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurrencyExchange.BusinessModels.Model
{
    public class CurrencyRates
    {

        public DateTime ExchangeDate { get; set; }
        public string Currency { get; set; }
        public string CurrencyCode { get; set; }
        public decimal CurrencyAmount { get; set; }
    }
}
