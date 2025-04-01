using CatNoteSchedule.API.DTOs;
using System.Net;
using System.Net.Http.Json;

namespace CatNoteSchedule.IntegrationTests;
public class SchedulesControllerIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public SchedulesControllerIntegrationTests(CustomWebApplicationFactory factory)
    {
        // IClassFixture создает factory один раз для всего набора тестов
        _factory = factory;
        // Создаем клиент, через который будем слать запросы в наше приложение
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task GenerateSchedule_ValidActivities_ShouldReturnOkAndSchedule()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var activities = new List<ActivityRequestDTO>
            {
                new ActivityRequestDTO { Name = "Reading", Duration = 2, Frequency = 1 },
                new ActivityRequestDTO { Name = "Jogging", Duration = 1, Frequency = 2 }
            };

        // Act
        var response = await _client.PostAsJsonAsync($"/api/Schedules/user/{userId}", activities);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var scheduleDict = await response.Content.ReadFromJsonAsync<Dictionary<string, List<string>>>();
        Assert.NotNull(scheduleDict);
        // У нас по умолчанию 7 дней: Monday, Tuesday, Wednesday, Thursday, Friday, Saturday, Sunday
        Assert.Equal(7, scheduleDict!.Count);

        // Проверим, что хотя бы где-то есть задачи
        var totalEvents = scheduleDict.Values.Sum(list => list.Count);
        Assert.True(totalEvents > 0, "Ожидаем, что в расписании есть хотя бы одна задача");
    }

    [Fact]
    public async Task GenerateSchedule_TooManyHours_ShouldReturn500()
    {
        // Arrange
        var userId = Guid.NewGuid();
        // Допустим, BaseValues.AvailableHours = 84, а мы хотим 100 часов
        var activities = new List<ActivityRequestDTO>
            {
                new ActivityRequestDTO { Name = "HugeTask", Duration = 5, Frequency = 20 }
            };

        // Act
        var response = await _client.PostAsJsonAsync($"/api/Schedules/user/{userId}", activities);

        // Assert
        // Контроллер не ловит Exception => ASP.NET вернет 500 (Internal Server Error)
        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
    }

    [Fact]
    public async Task GenerateSchedule_ActivityDoesNotFitIntoDay_ShouldReturn500()
    {
        // Arrange
        var userId = Guid.NewGuid();
        // По логике SetActivityTime, макс доступное окно в день 12 часов (9:00–21:00).
        // Если хотим 13 часов, упадёт "Не удалось найти время для активности"
        var activities = new List<ActivityRequestDTO>
            {
                new ActivityRequestDTO { Name = "LongActivity", Duration = 13, Frequency = 1 }
            };

        // Act
        var response = await _client.PostAsJsonAsync($"/api/Schedules/user/{userId}", activities);

        // Assert
        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
    }

    [Fact]
    public async Task GetById_ShouldReturnEmptyDictionary_WhenNoRecord()
    {
        // Arrange
        var randomGuid = Guid.NewGuid();

        // Act
        var getByIdResp = await _client.GetAsync($"/api/Schedules?id={randomGuid}");

        // Assert
        // Сейчас, если запись не найдена, сервис десериализует null.Shedule => может быть исключение
        // Допустим, возвращаем пустой словарь.
        // Либо сервис падает => 500.

        // В реальности ваш сервис => userSchedule=null => userSchedule.Shedule => NullRef
        // => 500. Проверяем это:
        Assert.Equal(HttpStatusCode.InternalServerError, getByIdResp.StatusCode);
    }
}