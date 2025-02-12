using Asp.Versioning;
using CurrencyExchange.BusinessLayer.Interfaces;
using CurrencyExchange.BusinessModels.Model;
using Microsoft.AspNetCore.Mvc;

namespace CurrencyExchange.Controllers
{
    //[ApiVersion("1.0")]
    //[Route("api/v{version:apiVersion}/convert")]
    //[ApiController]
    [ApiController]
    [Route("api/[controller]")]
    public class ExchangeRatesController : Controller
    {
        private readonly ICurrencyService _currencyService;

        public ExchangeRatesController(ICurrencyService currencyService)
        {
            _currencyService = currencyService;
        }

        [HttpPost("convert")]
        public async Task<IActionResult> ConvertCurrency([FromBody] CurrencyConversionRequest request)
        {
            var response = await _currencyService.ConvertCurrencyAsync(request);
            return Ok(response);
        }
    }
}

