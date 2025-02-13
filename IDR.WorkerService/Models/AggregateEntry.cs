namespace IDR.WorkerService.Models;

public class AggregateEntry
{
    public AggregateEntry(DateTime date, double volume)
    {
        PeriodTime = date;
        Sum = volume;
    }

    public DateTime PeriodTime { get; set; }
    public double Sum { get; set; }
}