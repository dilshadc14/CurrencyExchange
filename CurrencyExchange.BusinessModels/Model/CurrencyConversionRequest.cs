using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurrencyExchange.BusinessModels.Model
{
    public class CurrencyConversionRequest
    {
        public string From { get; set; }
        public string To { get; set; }
        public decimal Amount { get; set; }
    }
}
