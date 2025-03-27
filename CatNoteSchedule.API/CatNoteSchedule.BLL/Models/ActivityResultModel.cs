namespace CatNoteSchedule.BLL.Models;

public class ActivityResultModel
{
    public string Name { get; set; } = null!;
    public double Duration { get; set; }
    public int Frequency { get; set; }
    public TimeSpan ScheduledTime { get; set; }
}
