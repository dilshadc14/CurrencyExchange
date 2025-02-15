using System.Threading.RateLimiting;

namespace CurrencyExchange.Extensions
{
    public static class RateLimitingExtensions
    {
        public static IServiceCollection AddCustomRateLimiting(this IServiceCollection services)
        {
            services.AddRateLimiter(options =>
            {
                options.RejectionStatusCode = 429;
                options.OnRejected = (context, cancellationToken) =>
                {
                    context.HttpContext.Response.WriteAsync(
                        "Rate limit exceeded. Please try again later.", cancellationToken);
                    return new ValueTask();
                };

                options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
                    RateLimitPartition.GetFixedWindowLimiter(
                        context.Connection.RemoteIpAddress?.ToString() ?? "global",
                        _ => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = 10,
                            Window = TimeSpan.FromMinutes(1),
                            QueueLimit = 2
                        }));

                options.AddPolicy<string>("StrictLimit", context =>
                    RateLimitPartition.GetFixedWindowLimiter(
                        context.Connection.RemoteIpAddress?.ToString() ?? "strict",
                        _ => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = 5,
                            Window = TimeSpan.FromMinutes(1),
                            QueueLimit = 0
                        }));

                options.AddPolicy("LowTrafficPolicy", context =>
                    RateLimitPartition.GetFixedWindowLimiter(
                        context.Connection.RemoteIpAddress?.ToString() ?? "low-traffic",
                        _ => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = 10,
                            Window = TimeSpan.FromMinutes(1),
                            QueueLimit = 0
                        }));
            });

            return services;
        }
    }
}


//builder.Services.AddRateLimiter(options =>
//{
//    options.RejectionStatusCode = 429; 
//    options.OnRejected = (context, cancellationToken) =>
//    {
//        context.HttpContext.Response.WriteAsync("Rate limit exceeded. Please try again later.", cancellationToken);
//        return new ValueTask();
//    };

//    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
//        RateLimitPartition.GetFixedWindowLimiter(
//            partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? "global",
//            factory: _ => new FixedWindowRateLimiterOptions
//            {
//                PermitLimit = 10, 
//                Window = TimeSpan.FromMinutes(1), 
//                QueueLimit = 2
//            }
//        )
//    );
//    options.AddPolicy<string>("StrictLimit", context =>
//        RateLimitPartition.GetFixedWindowLimiter(
//            partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? "strict",
//            factory: _ => new FixedWindowRateLimiterOptions
//            {
//                PermitLimit = 5, 
//                Window = TimeSpan.FromMinutes(1), 
//                QueueLimit = 0 
//            }
//        )
//    );
//    options.AddPolicy<Guid>("UserLimit", context =>
//    {
//        var userId = context.User.FindFirst("UserId")?.Value;
//        return RateLimitPartition.GetFixedWindowLimiter(
//            partitionKey: userId != null ? Guid.Parse(userId) : Guid.NewGuid(),
//            factory: _ => new FixedWindowRateLimiterOptions
//            {
//                PermitLimit = 5,
//                Window = TimeSpan.FromMinutes(1)
//            }
//        );
//    });
//    options.AddPolicy("LowTrafficPolicy", context =>
//       RateLimitPartition.GetFixedWindowLimiter(
//           partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? "low-traffic",
//           factory: _ => new FixedWindowRateLimiterOptions
//           {
//               PermitLimit = 10,
//               Window = TimeSpan.FromMinutes(1),
//               QueueLimit = 0 
//           }
//       )
//   );
//});