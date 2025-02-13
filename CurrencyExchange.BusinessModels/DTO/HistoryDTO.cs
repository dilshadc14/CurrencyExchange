using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurrencyExchange.BusinessModels.DTO
{
    public class HistoricalExchangeRatesDTO
    {
        public decimal Amount { get; set; } 
        public string Base { get; set; } 
        public DateTime StartDate { get; set; } 
        public DateTime EndDate { get; set; } 
        public Dictionary<DateTime, Dictionary<string, decimal>> Rates { get; set; } // Dictionary of dates and exchange rates
    }
}
