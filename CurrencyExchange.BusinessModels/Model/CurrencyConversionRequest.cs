using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurrencyExchange.BusinessModels.Model
{
    public class CurrencyConversionRequest
    {
        [Required]
        public string From { get; set; }
        [Required]
        public string To { get; set; }
        public decimal Amount { get; set; }
    }
}
