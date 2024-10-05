namespace CatNoteSchedule.API.Models;

public class ActivityResult
{
    public string Name { get; set; } = null!;
    public double Duration { get; set; }
    public int Frequency { get; set; }
    public TimeSpan ScheduledTime { get; set; } // Время, назначенное на бэкенде
}
