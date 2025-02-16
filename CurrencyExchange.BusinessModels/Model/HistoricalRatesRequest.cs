using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurrencyExchange.BusinessModels.Model
{
    public class HistoricalRatesRequest
    {
        [Required]
        public string BaseCurrency { get; set; }
        [Required]
        [DataType(DataType.Date, ErrorMessage = "Invalid date format.")]
        [Display(Name ="Start Date")]
        public DateTime StartDate { get; set; }
        [DataType(DataType.Date, ErrorMessage = "Invalid date format.")]

        [Display(Name = "End Date")]
        public DateTime EndDate { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
