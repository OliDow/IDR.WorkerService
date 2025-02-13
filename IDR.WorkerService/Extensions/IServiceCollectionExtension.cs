using IDR.WorkerService.Providers;
using IDR.WorkerService.Services;
using Polly;
using Polly.Retry;
using Services;
using System.Diagnostics.CodeAnalysis;

namespace IDR.WorkerService.Extensions;

[ExcludeFromCodeCoverage]
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddTradeServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<ResiliencePipeline<IEnumerable<PowerTrade>>>(sp =>
        {
            var logger = sp.GetRequiredService<ILogger<TradesProvider>>();

            return new ResiliencePipelineBuilder<IEnumerable<PowerTrade>>()
                .AddRetry(new RetryStrategyOptions<IEnumerable<PowerTrade>>
                {
                    MaxRetryAttempts = 6,
                    BackoffType = DelayBackoffType.Exponential,
                    Delay = TimeSpan.FromSeconds(1),
                    OnRetry = args =>
                    {
                        logger.LogWarning(
                            args.Outcome.Exception,
                            "Retry attempt {RetryAttempt} after failure",
                            args.AttemptNumber);

                        return ValueTask.CompletedTask;
                    }
                })
                .Build();
        });

        services.AddSingleton<IPowerService, PowerService>();

        services.Configure<ConfigOptions>(configuration.GetSection("Config"));
        services.AddSingleton<ITradesProvider, TradesProvider>();
        services.AddSingleton<IAggregationService, AggregationService>();

        return services;
    }
}