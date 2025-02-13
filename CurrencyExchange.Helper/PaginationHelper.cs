using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CurrencyExchange.BusinessModels.Entities;
using CurrencyExchange.BusinessModels.Model;

namespace CurrencyExchange.Helper
{
    public static class PaginationHelper
    {
        public static PaginatedResponse<CurrencyRates> Paginate(List<CurrencyRates> data, int pageNumber, int pageSize)
        {
            var totalCount = data.Count;
            var paginatedData = data
                .OrderBy(x => x.ExchangeDate) 
                .Skip((pageNumber - 1) * pageSize) 
                .Take(pageSize) 
                .ToList();

            return new PaginatedResponse<CurrencyRates>
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount,
                Data = paginatedData
            };
        }
    }

}
