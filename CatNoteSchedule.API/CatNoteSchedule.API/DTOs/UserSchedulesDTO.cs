namespace CatNoteSchedule.API.DTOs;

public class UserSchedulesDTO
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Shedule { get; set; } = null!;
}
