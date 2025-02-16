using Asp.Versioning;
using CurrencyExchange.BusinessLayer.Common;
using CurrencyExchange.BusinessLayer.Interfaces;
using CurrencyExchange.BusinessModels.Model;
using CurrencyExchange.Helper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Swashbuckle.AspNetCore.Annotations;

namespace CurrencyExchange.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]

    public class ExchangeRatesController : Controller
    {
        private readonly ICurrencyService _currencyService;
        private readonly ICurrencyValidationService _currencysetting;

        public ExchangeRatesController(ICurrencyService currencyService, ICurrencyValidationService currencysetting)
        {
            _currencyService = currencyService;
            _currencysetting = currencysetting;
        }
        [SwaggerOperation(
  Summary = "Convert currency",
  Description = "Converts an amount from one currency to another using exchange rates."
)]
        [HttpPost("convert")]
        [Authorize(Policy = "UserOrAdmin")]
        [EnableRateLimiting("StrictLimit")]
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
        [SwaggerOperation(
Summary = "Convert currency",
Description = "Generate conversion rate using Basic currency  exchange rates."
)]
        [HttpGet("latest")]
        [Authorize(Policy = "UserOrAdmin")]
        [EnableRateLimiting("LowTrafficPolicy")]
        public async Task<IActionResult> GetLatestRates([FromQuery] string baseCurrency)
        {
            if (string.IsNullOrEmpty(baseCurrency))
            {
                return BadRequest("Currency code is required.");
            }
            var result = Validation.IsValidCurrencyCode(baseCurrency);
            if(!result.IsValid)
            {
                return BadRequest(result.ErrorMessage);
            }

            if (!_currencysetting.IsCurrencyAllowed(baseCurrency))
            {
                return BadRequest($"Currency '{baseCurrency}' is not allowed.");
            }

            var rates = await _currencyService.FetchLatestRatesAsync(baseCurrency);
            
            return Ok(rates);
        }
        [HttpGet("historical")]
        [Authorize(Policy = "AdminOnly")]
        [EnableRateLimiting("StrictLimit")]

        [SwaggerOperation(
Summary = "Convert currency",
Description = "Generate conversion rate using Basic currency  exchange rates. for  perticular time frame access restricted to admin"
)]
        public async Task<IActionResult> GetHistoricalRates([FromQuery] HistoricalRatesRequest request)
        {
            if (ModelState.IsValid)
            {
               
                var validation = Validation.IsValidCurrencyCode(request.BaseCurrency);
                if (!validation.IsValid)
                {
                    return BadRequest(validation.ErrorMessage);
                }
                if (!_currencysetting.IsCurrencyAllowed(request.BaseCurrency))
                {
                    return BadRequest("Conversion for TRY, PLN, THB, and MXN is not supported.");
                }
                if(request.StartDate> request.EndDate)
                {
                    return BadRequest("Start Date  must be less than or equal to End Date ");
                }


                var result = await _currencyService.GetHistoricalRatesAsync(request);
                return Ok(result);
            }
            else
            {
                var errors = ModelState.Keys
               .SelectMany(key => ModelState[key].Errors.Select(x => new { Field = key, Message = x.ErrorMessage }))
               .ToList();
                return BadRequest(new { Errors = errors });
            }
        }
    }
}

