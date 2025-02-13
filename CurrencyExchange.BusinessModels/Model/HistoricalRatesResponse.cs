using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;


namespace CurrencyExchange.BusinessModels.Model
{
    public class HistoricalRatesResponse: PaginatedResponse<CurrencyRates>
    {
        
        public string BaseCurrency { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
       
    }
   
   
}
