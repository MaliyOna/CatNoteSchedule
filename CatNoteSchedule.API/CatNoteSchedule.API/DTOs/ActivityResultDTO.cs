namespace CatNoteSchedule.API.DTOs;

public class ActivityResultDTO
{
    public string Name { get; set; } = null!;
    public double Duration { get; set; }
    public int Frequency { get; set; }
    public TimeSpan ScheduledTime { get; set; }
}
