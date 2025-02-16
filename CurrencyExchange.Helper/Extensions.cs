using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CurrencyExchange.Helper
{
   public static class Validation
    {
        

        public static (bool IsValid, string ErrorMessage) IsValidCurrencyCode(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return (false, "Input cannot be empty or whitespace.");

            if (!Regex.IsMatch(input, @"^[A-Za-z]{1,3}$"))
                return (false, "Invalid currency code. It must contain only letters (A-Z, a-z) with a maximum length of 3 characters.");

            return (true, "Valid currency code.");
        }
    }
}
