using Services;

namespace IDR.WorkerService.Providers;

public interface ITradesProvider
{
    Task<List<PowerTrade>> GetTradesAsync(DateTime dateTime);
}