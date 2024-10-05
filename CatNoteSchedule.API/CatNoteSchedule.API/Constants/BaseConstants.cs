using CatNoteSchedule.API.Models;

namespace CatNoteSchedule.API.Constants;

public static class BaseConstants
{
    public const double TotalWeekHours = 168;
    public const double SleepHours = 63;
    public const double EatingHours = 14;
    public const double AvailableHours = TotalWeekHours - SleepHours - EatingHours;

    public static Dictionary<string, HashSet<ActivityResult>> InitializeScheduleWithDefaults()
    {
        var schedule = new Dictionary<string, HashSet<ActivityResult>>();
        string[] days = { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday" };

        foreach (var day in days)
        {
            schedule[day] = new HashSet<ActivityResult>
            {
                new ActivityResult { Name = "Сон", ScheduledTime = TimeSpan.FromHours(0), Duration = 9 },
                new ActivityResult { Name = "Завтрак", ScheduledTime = TimeSpan.FromHours(9), Duration = 1 },
                new ActivityResult { Name = "Обед", ScheduledTime = TimeSpan.FromHours(12), Duration = 1 },
                new ActivityResult { Name = "Перекус", ScheduledTime = TimeSpan.FromHours(16), Duration = 1 },
                new ActivityResult { Name = "Ужин", ScheduledTime = TimeSpan.FromHours(20), Duration = 1 }
            };
        }

        return schedule;
    }
}
