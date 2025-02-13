using Services;

namespace IDR.WorkerService.Services;

public class AggregationService : IAggregationService
{
    public Dictionary<int, AggregateEntry> AggregateTradeData(IEnumerable<PowerTrade> trades)
    {
        var aggregatedData = new Dictionary<int, AggregateEntry>();
        foreach (var trade in trades)
        {
            var periodOffset = -1; // Initial Entry is 23:00
            foreach (var period in trade.Periods)
            {
                if (aggregatedData.TryGetValue(period.Period, out var entry))
                {
                    entry.Sum += period.Volume;
                }
                else
                {
                    entry = new AggregateEntry(trade.Date.AddHours(periodOffset), period.Volume);
                    aggregatedData.Add(period.Period, entry);
                    periodOffset++;
                }
            }
        }

        return aggregatedData;
    }
}