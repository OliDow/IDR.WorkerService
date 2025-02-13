using Services;

namespace IDR.WorkerService.Services;

public interface IAggregationService
{
    Dictionary<int, AggregateEntry> AggregateTradeData(IEnumerable<PowerTrade> trades);
}