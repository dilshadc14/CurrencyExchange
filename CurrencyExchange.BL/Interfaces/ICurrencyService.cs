
using System.Threading.Tasks;
using CurrencyExchange.BusinessModels.Model;

namespace CurrencyExchange.BusinessLayer.Interfaces
{
    public interface ICurrencyService
    {
        Task<CurrencyConversionResponse> ConvertCurrencyAsync(CurrencyConversionRequest request);
    }
}
