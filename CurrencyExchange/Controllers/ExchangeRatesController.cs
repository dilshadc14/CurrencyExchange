using Asp.Versioning;
using CurrencyExchange.BusinessLayer.Common;
using CurrencyExchange.BusinessLayer.Interfaces;
using CurrencyExchange.BusinessModels.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CurrencyExchange.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
  //  [Authorize]
    public class ExchangeRatesController : Controller
    {
        private readonly ICurrencyService _currencyService;
        private readonly ICurrencyValidationService _currencysetting;

        public ExchangeRatesController(ICurrencyService currencyService, ICurrencyValidationService currencysetting)
        {
            _currencyService = currencyService;
            _currencysetting = currencysetting;
        }

        [HttpPost("convert")]
        public async Task<IActionResult> ConvertCurrency([FromBody] CurrencyConversionRequest request)
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(request.From) || string.IsNullOrEmpty(request.To))
                {
                    return BadRequest("Please provide the currency to exchange");
                }
                if (!_currencysetting.IsCurrencyAllowed(request.From) )
                {
                    return BadRequest($"Currency '{request.From}'  is not allowed.");
                }
                if (!_currencysetting.IsCurrencyAllowed(request.To))
                {
                    return BadRequest($"Currency '{request.To}'  is not allowed.");
                }
                var response = await _currencyService.ConvertCurrencyAsync(request);
                return Ok(response);
            }
            else
            {
                var errors = ModelState.Keys
               .SelectMany(key => ModelState[key].Errors.Select(x => new { Field = key, Message = x.ErrorMessage }))
               .ToList();
                return BadRequest(new { Errors = errors });
            }
        }

        [HttpGet("latest")]
        public async Task<IActionResult> GetLatestRates([FromQuery] string baseCurrency)
        {
            if (string.IsNullOrEmpty(baseCurrency))
            {
                return BadRequest("Currency code is required.");
            }

            if (!_currencysetting.IsCurrencyAllowed(baseCurrency))
            {
                return BadRequest($"Currency '{baseCurrency}' is not allowed.");
            }

            var rates = await _currencyService.FetchLatestRatesAsync(baseCurrency);
            
            return Ok(rates);
        }
        [HttpGet("historical")]
        public async Task<IActionResult> GetHistoricalRates([FromQuery] HistoricalRatesRequest request)
        {
            if (string.IsNullOrEmpty(request.BaseCurrency))
            {
                return BadRequest("Please provide the currency to exchange");
            }
            if (!_currencysetting.IsCurrencyAllowed(request.BaseCurrency))
            {
                return BadRequest("Conversion for TRY, PLN, THB, and MXN is not supported.");
            }
            var result = await _currencyService.GetHistoricalRatesAsync(request);
            return Ok(result);
        }
    }
}

