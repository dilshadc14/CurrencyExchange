using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurrencyExchange.BusinessModels.Model
{
    public class BaseModelResponse
    {
        public bool Status {  get; set; }
        public string Message { get; set; }
    }
    public enum Status
    {
        Success, Error,
        [Description("Invalid request")]
        InvalidRequest
    }

}
