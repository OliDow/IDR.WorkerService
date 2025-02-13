using IDR.WorkerService.Extensions;
using IDR.WorkerService.Providers;
using IDR.WorkerService.Services;
using Microsoft.Extensions.Options;
using System.Text;

namespace IDR.WorkerService;

public class Worker : BackgroundService
{
    private readonly ITradesProvider _tradesProvider;
    private readonly IAggregationService _aggregationService;
    private readonly ILogger<Worker> _logger;
    private readonly ConfigOptions _configOptions;

    public Worker(
        ITradesProvider tradesProvider,
        IAggregationService aggregationService,
        IOptions<ConfigOptions> options,
        ILogger<Worker> logger)
    {
        _tradesProvider = tradesProvider ?? throw new ArgumentNullException(nameof(tradesProvider));
        _aggregationService = aggregationService ?? throw new ArgumentNullException(nameof(aggregationService));
        _configOptions = options.Value ?? throw new ArgumentNullException(nameof(options));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var executionFrequency = TimeSpan.FromMinutes(_configOptions.Frequency);

        while (!stoppingToken.IsCancellationRequested)
        {
            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation("Execution Frequency: {Frequency} Minute(s)", _configOptions.Frequency);
                _logger.LogInformation("Execution Started : {time}", DateTimeOffset.Now);

                _logger.LogInformation("Fetching Trades: {time}", DateTimeOffset.Now);
                var trades = await _tradesProvider.GetTradesAsync(DateTime.Now);

                _logger.LogInformation("Aggregating Data: {time}", DateTimeOffset.Now);
                var aggregatedData = _aggregationService.AggregateTradeData(trades);

                // Create CSV Data (abstract?)
                StringBuilder sb = new();
                sb.AppendLine("Local Time,Volume");
                foreach (var entry in aggregatedData)
                {
                    sb.AppendLine($"{entry.Value.PeriodTime.ToString("t")}, {entry.Value.Sum}");
                }

                // Persist to file
                var filenameDate = DateTime.Now.ToString("yyyMMdd_HHmm");
                var outputLocation = $"{_configOptions.OutputDirectory}/PowerPosition_{filenameDate}.csv";

                _logger.LogInformation("Persisting output to: {outputLocation}", outputLocation);
                await using var writer = new StreamWriter(outputLocation);
                await writer.WriteLineAsync(sb.ToString());

                _logger.LogInformation("Execution Complete : {time}", DateTimeOffset.Now);
            }

            await Task.Delay(executionFrequency, stoppingToken);
        }
    }
}