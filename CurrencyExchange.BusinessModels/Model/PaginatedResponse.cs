using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurrencyExchange.BusinessModels.Model
{
    public class PaginatedResponse<T> where T : class
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public List<T> Data { get; set; }

        public bool IsError { get; set; } = false;

        public string ErrorMessage {  get; set; }

    }
}
