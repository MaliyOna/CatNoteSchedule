namespace CatNoteSchedule.API.Models;

public class ActivityRequest
{
    public string Name { get; set; } = null!;
    public double Duration { get; set; }
    public int Frequency { get; set; }
}
