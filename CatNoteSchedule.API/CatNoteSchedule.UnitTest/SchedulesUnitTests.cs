using CatNoteSchedule.BLL.Abstractions;
using CatNoteSchedule.BLL.Models;
using CatNoteSchedule.BLL.Services;
using CatNoteSchedule.DAL.Abstractions;
using CatNoteSchedule.DAL.Models;
using Moq;
using Newtonsoft.Json;

namespace CatNoteSchedule.UnitTest;

public class SchedulesUnitTests
{
    private readonly Mock<IUserSchedulesRepository> _userSchedulesRepoMock;
    private readonly ISchedulesService _schedulesService;

    public SchedulesUnitTests()
    {
        _userSchedulesRepoMock = new Mock<IUserSchedulesRepository>(MockBehavior.Strict);
        _schedulesService = new SchedulesService(_userSchedulesRepoMock.Object);
    }

    [Fact]
    public async Task GenerateSchedule_ShouldThrowException_WhenActivitiesExceedAvailableHours()
    {
        // Arrange
        var activities = new List<ActivityRequestModel>
            {
                new ActivityRequestModel { Name = "Big Task", Duration = 5, Frequency = 20 },
            };
        var userId = Guid.NewGuid();

        // Act + Assert
        await Assert.ThrowsAsync<Exception>(() =>
            _schedulesService.GenerateSchedule(activities, userId, CancellationToken.None));

        _userSchedulesRepoMock.Verify(repo => repo.Add(It.IsAny<UserSсhedules>(), It.IsAny<CancellationToken>()),
                                      Times.Never);
    }

    [Fact]
    public async Task GenerateSchedule_ShouldSaveScheduleAndReturnIt_WhenActivitiesAreValid()
    {
        // Arrange
        var userId = Guid.NewGuid();

        var activities = new List<ActivityRequestModel>
            {
                new ActivityRequestModel { Name = "Jogging", Duration = 1, Frequency = 2 },
                new ActivityRequestModel { Name = "Reading", Duration = 2, Frequency = 1 }
            };

        _userSchedulesRepoMock
            .Setup(repo => repo.Add(It.IsAny<UserSсhedules>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _schedulesService.GenerateSchedule(activities, userId, CancellationToken.None);

        // Assert

        _userSchedulesRepoMock.Verify(repo => repo.Add(It.Is<UserSсhedules>(x =>
              x.UserId == userId &&
              JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(x.Shedule).Count == 7
          ),
          It.IsAny<CancellationToken>()),
        Times.Once);

        Assert.Equal(7, result.Count);

        var totalTasks = result.Values.SelectMany(list => list).Count();
        Assert.True(totalTasks > 0, "Schedule should have tasks distributed");
    }

    [Fact]
    public async Task GetByUserId_ShouldReturnNull_WhenNoScheduleFound()
    {
        // Arrange
        var userId = Guid.NewGuid();

        _userSchedulesRepoMock
            .Setup(repo => repo.GetByUserId(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserSсhedules?)null);

        // Act
        var result = await _schedulesService.GetByUserId(userId, CancellationToken.None);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByUserId_ShouldReturnUserSchedulesModel_WhenRecordExists()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var existingRecord = new UserSсhedules
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Shedule = "{\"Monday\":[\"Task1\"],\"Tuesday\":[]}"
        };

        _userSchedulesRepoMock
            .Setup(repo => repo.GetByUserId(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingRecord);

        // Act
        var result = await _schedulesService.GetByUserId(userId, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(existingRecord.Id, result!.Id);
        Assert.Equal(existingRecord.UserId, result.UserId);
        Assert.Equal(existingRecord.Shedule, result.Shedule);
    }

    [Fact]
    public async Task GetAll_ShouldReturnListOfUserSchedulesModels()
    {
        // Arrange
        var listFromRepo = new List<UserSсhedules>
            {
                new UserSсhedules { Id = Guid.NewGuid(), UserId = Guid.NewGuid(), Shedule = "{}" },
                new UserSсhedules { Id = Guid.NewGuid(), UserId = Guid.NewGuid(), Shedule = "{\"Monday\":[\"Task1\"]}" },
            };

        _userSchedulesRepoMock
            .Setup(repo => repo.GetAll(It.IsAny<CancellationToken>()))
            .ReturnsAsync(listFromRepo);

        // Act
        var result = await _schedulesService.GetAll(CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Equal(listFromRepo[0].Id, result[0].Id);
        Assert.Equal(listFromRepo[1].Id, result[1].Id);
    }

    [Fact]
    public async Task GetById_ShouldReturnDictionary_WhenRecordFound()
    {
        // Arrange
        var scheduleId = Guid.NewGuid();
        var userSchedules = new UserSсhedules
        {
            Id = scheduleId,
            UserId = Guid.NewGuid(),
            Shedule = "{\"Monday\":[\"Task1\", \"Task2\"],\"Tuesday\":[]}"
        };

        _userSchedulesRepoMock
            .Setup(repo => repo.GetById(scheduleId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(userSchedules);

        // Act
        var dictionary = await _schedulesService.GetById(scheduleId, CancellationToken.None);

        // Assert
        Assert.NotNull(dictionary);
        Assert.True(dictionary.ContainsKey("Monday"));
        Assert.Equal(2, dictionary["Monday"]!.Count);
    }
}
