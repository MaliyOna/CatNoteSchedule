namespace CatNoteSchedule.API.DTOs;

public class ActivityRequestDTO
{
    public string Name { get; set; } = null!;
    public double Duration { get; set; }
    public int Frequency { get; set; }
}
