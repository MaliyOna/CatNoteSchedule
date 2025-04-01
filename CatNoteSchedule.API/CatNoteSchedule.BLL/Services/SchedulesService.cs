using CatNoteSchedule.BLL.Abstractions;
using CatNoteSchedule.BLL.Constants;
using CatNoteSchedule.BLL.Models;
using CatNoteSchedule.DAL.Abstractions;
using CatNoteSchedule.DAL.Models;
using Newtonsoft.Json;

namespace CatNoteSchedule.BLL.Services;

public class SchedulesService : ISchedulesService
{
    private readonly IUserSchedulesRepository userSchedulesRepository;

    public SchedulesService(IUserSchedulesRepository userSchedulesRepository)
    {
        this.userSchedulesRepository = userSchedulesRepository;
    }

    public async Task<Dictionary<string, List<string>>> GenerateSchedule(List<ActivityRequestModel> activities, Guid userId, CancellationToken cancellationToken)
    {
        ValidateActivities(activities);

        var activityList = activities.Select(a => new ActivityResultModel
        {
            Name = a.Name,
            Duration = a.Duration,
            Frequency = a.Frequency
        }).ToList();

        var schedule = BaseValues.InitializeScheduleWithDefaults();

        DistributeActivitiesEqually(activityList, schedule);

        OptimizeSchedule(schedule);

        var scheduleForApi = ConvertScheduleToApiFormat(schedule);

        var userShedule = new UserSсhedules()
        {
            UserId = userId,
            Shedule = JsonConvert.SerializeObject(scheduleForApi)
        };

        await userSchedulesRepository.Add(userShedule, cancellationToken);

        return scheduleForApi;
    }

    public async Task<UserSchedulesModel?> GetByUserId(Guid userId, CancellationToken cancellationToken)
    {
        var userSchedule = await userSchedulesRepository.GetByUserId(userId, cancellationToken);

        return userSchedule == null
            ? null
            : new()
            {
                Id = userSchedule.Id,
                UserId = userSchedule.UserId,
                Shedule = userSchedule.Shedule
            };
    }

    public async Task<List<UserSchedulesModel>> GetAll(CancellationToken cancellationToken)
    {
        var userShedules = await userSchedulesRepository.GetAll(cancellationToken);

        return userShedules.Select(x => new UserSchedulesModel
        {
            Id = x.Id,
            UserId = x.UserId,
            Shedule = x.Shedule
        }).ToList();
    }

    public async Task<Dictionary<string, List<string>?>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var userShedule = await userSchedulesRepository.GetById(id, cancellationToken);

        var scheduleDictionary = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(userShedule.Shedule);

        return scheduleDictionary;
    }

    private void ValidateActivities(List<ActivityRequestModel> activities)
    {
        double totalRequiredHours = 0;
        foreach (var activity in activities)
        {
            totalRequiredHours += activity.Duration * activity.Frequency;
        }

        if (totalRequiredHours > BaseValues.AvailableHours)
        {
            throw new Exception("Общее количество требуемых часов активности превышает доступное время в неделю.");
        }
    }

    private void DistributeActivitiesEqually(List<ActivityResultModel> activities, Dictionary<string, HashSet<ActivityResultModel>> schedule)
    {
        var rnd = new Random();
        var days = schedule.Keys.ToList();

        foreach (var activity in activities)
        {
            var availableDays = new Queue<string>(days.OrderBy(x => rnd.Next()));
            for (int i = 0; i < activity.Frequency; i++)
            {
                if (availableDays.Count == 0)
                {
                    availableDays = new Queue<string>(days.OrderBy(x => rnd.Next()));
                }

                var day = availableDays.Dequeue();
                SetActivityTime(activity, schedule[day]);
                schedule[day].Add(new ActivityResultModel
                {
                    Name = activity.Name,
                    Duration = activity.Duration,
                    Frequency = activity.Frequency,
                    ScheduledTime = activity.ScheduledTime
                });
            }
        }
    }

    private void SetActivityTime(ActivityResultModel activity, HashSet<ActivityResultModel> dayActivities)
    {
        var startTime = TimeSpan.FromHours(9);
        var endTime = TimeSpan.FromHours(21);
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
                startTime += TimeSpan.FromMinutes(60);
            }
        }

        if (!placed)
        {
            throw new Exception("Не удалось найти время для активности в этот день.");
        }
    }

    private void OptimizeSchedule(Dictionary<string, HashSet<ActivityResultModel>> schedule)
    {
        var activityDays = new Dictionary<string, List<string>>();

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

        foreach (var activity in activityDays.Keys)
        {
            var days = activityDays[activity];
            for (int i = 0; i < days.Count - 1; i++)
            {
                if (days[i + 1] == days[i])
                {
                    string newDay = schedule.Keys.Except(days).FirstOrDefault();
                    if (newDay != null)
                    {
                        var activityToMove = schedule[days[i]].FirstOrDefault(a => a.Name == activity);
                        if (activityToMove != null)
                        {
                            schedule[days[i]].Remove(activityToMove);
                            schedule[newDay].Add(activityToMove);
                            days[i] = newDay;
                        }
                    }
                }
            }
        }
    }

    private Dictionary<string, List<string>> ConvertScheduleToApiFormat(Dictionary<string, HashSet<ActivityResultModel>> schedule)
    {
        var convertedSchedule = new Dictionary<string, List<string>>();

        foreach (var day in schedule.Keys)
        {
            var dayEvents = schedule[day]
                .OrderBy(a => a.ScheduledTime)
                .Select(a => $"{a.Name} в {a.ScheduledTime:hh\\:mm}")
                .ToList();

            convertedSchedule[day] = dayEvents;
        }

        return convertedSchedule;
    }
}
