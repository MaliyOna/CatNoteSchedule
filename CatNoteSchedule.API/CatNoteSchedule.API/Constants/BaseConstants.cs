using System.Collections.Generic;

namespace CatNoteSchedule.API.Constants;

public static class BaseConstants
{
    public const double TotalWeekHours = 168;
    public const double SleepHours = 63;
    public const double EatingHours = 14;
    public const double AvailableHours = TotalWeekHours - SleepHours - EatingHours;

    public static Dictionary<string, HashSet<string>> InitializeScheduleWithHashSet()
    {
        return new Dictionary<string, HashSet<string>>
        {
            { "Monday", new HashSet<string>() },
            { "Tuesday", new HashSet<string>() },
            { "Wednesday", new HashSet<string>() },
            { "Thursday", new HashSet<string>() },
            { "Friday", new HashSet<string>() },
            { "Saturday", new HashSet<string>() },
            { "Sunday", new HashSet<string>() }
        };
    }
}
