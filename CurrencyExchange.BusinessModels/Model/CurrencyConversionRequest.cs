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
        [Required(ErrorMessage ="From Currency Is Required")]
        [MaxLength(3, ErrorMessage = "From Currency must be exactly 3 characters, e.g., EUR")]
        [RegularExpression(@"^[A-Za-z]+$", ErrorMessage = "Only letters allowed.")]
        public string From { get; set; }
       
        [MaxLength(3, ErrorMessage = "To Currency must be exactly 3 characters, e.g., EUR")]
        [Required(ErrorMessage = "To Currency Is Required")]
        public string To { get; set; }

        [Required(ErrorMessage = "Amount is required")]
        public decimal Amount { get; set; }
    }
}
