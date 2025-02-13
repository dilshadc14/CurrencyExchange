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
        public string StartDate { get; set; }
        public string EndDate { get; set; }
       
    }
   
   
}
