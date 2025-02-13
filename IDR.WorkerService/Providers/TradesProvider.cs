using Polly;
using Services;

namespace IDR.WorkerService.Providers;

public class TradesProvider : ITradesProvider
{
    private readonly IPowerService _powerService;
    private readonly ResiliencePipeline<IEnumerable<PowerTrade>> _retryPolicy;

    public TradesProvider(IPowerService powerService, ResiliencePipeline<IEnumerable<PowerTrade>> retryPolicy)
    {
        _powerService = powerService ?? throw new ArgumentNullException(nameof(powerService));
        _retryPolicy = retryPolicy ?? throw new ArgumentNullException(nameof(retryPolicy));
    }

    public async Task<List<PowerTrade>> GetTradesAsync(DateTime dateTime)
    {
        var trades = await _retryPolicy.ExecuteAsync(async _ => await _powerService.GetTradesAsync(dateTime));

        return trades.ToList();
    }
}