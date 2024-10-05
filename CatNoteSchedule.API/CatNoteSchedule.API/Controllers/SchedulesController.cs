using Microsoft.AspNetCore.Mvc;
using CatNoteSchedule.API.Models;
using CatNoteSchedule.API.Constants;

namespace CatNoteSchedule.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SchedulesController : ControllerBase
{

    [HttpPost]
    public async Task<Dictionary<string, List<string>>> GenerateSchedule(List<ActivityModel> activities)
    {
        ValidateActivities(activities);
        var schedule = BaseConstants.InitializeScheduleWithHashSet();

        DistributeActivitiesEqually(activities, schedule);

        OptimizeSchedule(schedule);

        return ConvertScheduleToApiFormat(schedule);
    }

    private Dictionary<string, List<string>> ConvertScheduleToApiFormat(Dictionary<string, HashSet<string>> schedule)
    {
        var convertedSchedule = new Dictionary<string, List<string>>();
        foreach (var day in schedule.Keys)
        {
            convertedSchedule[day] = schedule[day].ToList();
        }
        return convertedSchedule;
    }

    private void ValidateActivities(List<ActivityModel> activities)
    {
        double totalRequiredHours = 0;
        foreach (var activity in activities)
        {
            totalRequiredHours += activity.Duration * activity.Frequency;
        }

        if (totalRequiredHours > BaseConstants.AvailableHours)
        {
            throw new Exception("Общее количество требуемых часов активности превышает доступное время в неделю");
        }
    }

    private void DistributeActivitiesEqually(List<ActivityModel> activities, Dictionary<string, HashSet<string>> schedule)
    {
        var rnd = new Random();
        var days = schedule.Keys.ToList();

        foreach (var activity in activities)
        {
            var shuffledDays = days.OrderBy(x => rnd.Next()).ToList();
            for (int i = 0; i < activity.Frequency; i++)
            {
                var day = shuffledDays.OrderBy(d => schedule[d].Count).First(); //тут размещаются занятия с учетом текущей загрузки дня
                schedule[day].Add(activity.Name);
                shuffledDays = shuffledDays.OrderBy(x => schedule[x].Count).ThenBy(x => rnd.Next()).ToList(); // меняем день для следующего распределения
            }
        }
    }

    private void OptimizeSchedule(Dictionary<string, HashSet<string>> schedule)
    {
        bool improvements;
        do
        {
            improvements = false;
            var dayKeys = schedule.Keys.ToList();  // Получаем список всех дней один раз
            for (int i = 0; i < dayKeys.Count; i++)
            {
                string currentDay = dayKeys[i];
                string nextDay = dayKeys[(i + 1) % dayKeys.Count];

                var commonActivities = schedule[currentDay].Intersect(schedule[nextDay]).ToList();

                foreach (var activity in commonActivities)
                {
                    // Найти подходящий день для перестановки активности
                    string targetDay = dayKeys.FirstOrDefault(day => day != currentDay && day != nextDay && !schedule[day].Contains(activity));
                    if (targetDay != null)
                    {
                        schedule[currentDay].Remove(activity);
                        schedule[targetDay].Add(activity);
                        improvements = true;
                    }
                }
            }
        } while (improvements); // Продолжаем, пока есть улучшения
    }
}
