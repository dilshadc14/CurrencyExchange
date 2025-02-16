using System.Security.Claims;
using Microsoft.Extensions.Options;

namespace CurrencyExchange.MiddleWares
{
    public class ClientIpMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ClientIpSettings _clientIpSettings;
        public ClientIpMiddleware(RequestDelegate next, IOptions<ClientIpSettings> clientIpSettings)
        {
            _next = next;
            _clientIpSettings = clientIpSettings.Value;
        }

        public async Task InvokeAsync(HttpContext context)
        {

            var clientIp = context.Connection.RemoteIpAddress?.ToString();
            if (context.Request.Headers.ContainsKey("X-Forwarded-For"))
            {
                clientIp = context.Request.Headers["X-Forwarded-For"].ToString();
            }
            if (!string.IsNullOrEmpty(clientIp) && !System.Net.IPAddress.TryParse(clientIp, out _))
            {//validating ip  format
                clientIp = _clientIpSettings.DefaultIp;
            }
            if (string.IsNullOrEmpty(clientIp))
            {
                clientIp = _clientIpSettings.DefaultIp;
            }
           

            if (!string.IsNullOrEmpty(clientIp))
            {
             
                var claimsIdentity = context.User.Identity as ClaimsIdentity;
                claimsIdentity?.AddClaim(new Claim("client_ip", clientIp));
            }
           


            await _next(context);
        }
    }
}
