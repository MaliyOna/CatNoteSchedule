namespace CatNoteSchedule.API.Models;

public class ActivityModel
{
    public string Name { get; set; } = null!;
    public double Duration { get; set; } // В часах
    public int Frequency { get; set; }
}
