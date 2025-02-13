using IDR.WorkerService;
using IDR.WorkerService.Extensions;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        services.AddTradeServices(hostContext.Configuration);
        services.AddHostedService<Worker>();
    })
    .Build();

await host.RunAsync();