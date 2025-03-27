namespace CatNoteSchedule.BLL.Models;

public class UserSchedulesModel
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Shedule { get; set; } = null!;
}
