using Microsoft.AspNetCore.Mvc;
using CatNoteSchedule.API.Models;
using CatNoteSchedule.API.Constants;

namespace CatNoteSchedule.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SchedulesController : ControllerBase
{

    [HttpPost]
    public async Task<ActionResult<Dictionary<string, List<string>>>> GenerateSchedule(List<ActivityRequest> activities)
    {
        try
        {
            ValidateActivities(activities);

            var activityList = activities.Select(a => new ActivityResult
            {
                Name = a.Name,
                Duration = a.Duration,
                Frequency = a.Frequency
            }).ToList();

            var schedule = BaseConstants.InitializeScheduleWithDefaults();

            DistributeActivitiesEqually(activityList, schedule);

            OptimizeSchedule(schedule);

            var scheduleForApi = ConvertScheduleToApiFormat(schedule);

            return scheduleForApi;
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    private void ValidateActivities(List<ActivityRequest> activities)
    {
        double totalRequiredHours = 0;
        foreach (var activity in activities)
        {
            totalRequiredHours += activity.Duration * activity.Frequency;
        }

        if (totalRequiredHours > BaseConstants.AvailableHours)
        {
            throw new Exception("Общее количество требуемых часов активности превышает доступное время в неделю.");
        }
    }

    private void DistributeActivitiesEqually(List<ActivityResult> activities, Dictionary<string, HashSet<ActivityResult>> schedule)
    {
        var rnd = new Random();
        var days = schedule.Keys.ToList();

        foreach (var activity in activities)
        {
            var availableDays = new Queue<string>(days.OrderBy(x => rnd.Next())); // Случайное перемешивание дней
            for (int i = 0; i < activity.Frequency; i++)
            {
                if (availableDays.Count == 0)
                {
                    availableDays = new Queue<string>(days.OrderBy(x => rnd.Next())); // Перемешать снова, если закончились дни
                }

                var day = availableDays.Dequeue();
                SetActivityTime(activity, schedule[day]);
                schedule[day].Add(new ActivityResult
                {
                    Name = activity.Name,
                    Duration = activity.Duration,
                    Frequency = activity.Frequency,
                    ScheduledTime = activity.ScheduledTime
                });
            }
        }
    }

    private void SetActivityTime(ActivityResult activity, HashSet<ActivityResult> dayActivities)
    {
        var startTime = TimeSpan.FromHours(9); // Начало дня
        var endTime = TimeSpan.FromHours(21); // Конец дня
        bool placed = false;

        while (!placed && startTime + TimeSpan.FromHours(activity.Duration) <= endTime)
        {
            var endTimeActivity = startTime + TimeSpan.FromHours(activity.Duration);
            if (!dayActivities.Any(a => a.ScheduledTime < endTimeActivity && a.ScheduledTime + TimeSpan.FromHours(a.Duration) > startTime))
            {
                activity.ScheduledTime = startTime;
                placed = true;
            }
            else
            {
                startTime += TimeSpan.FromMinutes(60); // Переход к следующему возможному времени начала
            }
        }

        if (!placed)
        {
            throw new Exception("Не удалось найти время для активности в этот день.");
        }
    }

    private void OptimizeSchedule(Dictionary<string, HashSet<ActivityResult>> schedule)
    {
        var activityDays = new Dictionary<string, List<string>>();

        // Сбор информации о днях, в которые запланированы активности
        foreach (var day in schedule.Keys)
        {
            foreach (var activity in schedule[day])
            {
                if (!activityDays.ContainsKey(activity.Name))
                {
                    activityDays[activity.Name] = new List<string>();
                }
                activityDays[activity.Name].Add(day);
            }
        }

        // Попытка уменьшить количество последовательных дней для каждой активности
        foreach (var activity in activityDays.Keys)
        {
            var days = activityDays[activity];
            for (int i = 0; i < days.Count - 1; i++)
            {
                if (days[i + 1] == days[i])
                {
                    // Найти новый день для перемещения активности
                    string newDay = schedule.Keys.Except(days).FirstOrDefault();
                    if (newDay != null)
                    {
                        var activityToMove = schedule[days[i]].FirstOrDefault(a => a.Name == activity);
                        if (activityToMove != null)
                        {
                            schedule[days[i]].Remove(activityToMove);
                            schedule[newDay].Add(activityToMove);
                            days[i] = newDay; // Обновить информацию о дне
                        }
                    }
                }
            }
        }
    }

    private Dictionary<string, List<string>> ConvertScheduleToApiFormat(Dictionary<string, HashSet<ActivityResult>> schedule)
    {
        var convertedSchedule = new Dictionary<string, List<string>>();

        foreach (var day in schedule.Keys)
        {
            // Собираем все активности в этот день, сортируем по времени начала и форматируем строку вывода
            var dayEvents = schedule[day]
                .OrderBy(a => a.ScheduledTime)  // Сортировка по времени начала
                .Select(a => $"{a.Name} at {a.ScheduledTime:hh\\:mm}")  // Форматирование вывода
                .ToList();

            convertedSchedule[day] = dayEvents;
        }

        return convertedSchedule;
    }
}
