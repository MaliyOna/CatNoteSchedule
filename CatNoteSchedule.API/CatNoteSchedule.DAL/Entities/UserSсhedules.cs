namespace CatNoteSchedule.DAL.Models;

public class UserSсhedules
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Shedule { get; set; } = null!;
}
