using CatNoteSchedule.BLL.Models;

namespace CatNoteSchedule.BLL.Constants;
public static class BaseValues
{
    public const double TotalWeekHours = 168;
    public const double SleepHours = 63;
    public const double EatingHours = 14;
    public const double AvailableHours = TotalWeekHours - SleepHours - EatingHours;

    public static Dictionary<string, HashSet<ActivityResultModel>> InitializeScheduleWithDefaults()
    {
        var schedule = new Dictionary<string, HashSet<ActivityResultModel>>();
        string[] days = { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday" };

        foreach (var day in days)
        {
            schedule[day] = new HashSet<ActivityResultModel>
            {
                new() { Name = "Сон", ScheduledTime = TimeSpan.FromHours(0), Duration = 9 },
                new() { Name = "Завтрак", ScheduledTime = TimeSpan.FromHours(9), Duration = 1 },
                new() { Name = "Обед", ScheduledTime = TimeSpan.FromHours(12), Duration = 1 },
                new() { Name = "Перекус", ScheduledTime = TimeSpan.FromHours(16), Duration = 1 },
                new() { Name = "Ужин", ScheduledTime = TimeSpan.FromHours(20), Duration = 1 }
            };
        }

        return schedule;
    }
}

