using CatNoteSchedule.API.DTOs;
using CatNoteSchedule.BLL.Abstractions;
using CatNoteSchedule.BLL.Models;
using Microsoft.AspNetCore.Mvc;

namespace CatNoteSchedule.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SchedulesController : ControllerBase
{
    private readonly ISchedulesService schedulesService;

    public SchedulesController(ISchedulesService schedulesService)
    {
        this.schedulesService = schedulesService;
    }

    [HttpPost("user/{userId}")]
    public async Task<Dictionary<string, List<string>>> GenerateSchedule(List<ActivityRequestDTO> activities, [FromQuery] Guid userId, CancellationToken cancellationToken)
    {
        var activitiesModel = activities.Select(x => new ActivityRequestModel
        {
            Name = x.Name,
            Duration = x.Duration,
            Frequency = x.Frequency
        }).ToList();

        return await schedulesService.GenerateSchedule(activitiesModel, userId, cancellationToken);
    }

    [HttpGet("user/{userId}")]
    public async Task<UserSchedulesDTO> GetByUserId([FromQuery] Guid userId, CancellationToken cancellationToken)
    {
        var userSchedule = await schedulesService.GetByUserId(userId, cancellationToken);

        return new()
        {
            Id = userSchedule.Id,
            Shedule = userSchedule.Shedule,
            UserId = userSchedule.UserId
        };
    }

    [HttpGet("{id}")]
    public async Task<Dictionary<string, List<string>>> GetById([FromQuery] Guid id, CancellationToken cancellationToken)
    {
        var schedule = await schedulesService.GetById(id, cancellationToken);

        return schedule;
    }

    [HttpGet]
    public async Task<List<UserSchedulesDTO>> GetAll(CancellationToken cancellationToken)
    {
        var userSchedule = await schedulesService.GetAll(cancellationToken);

        return userSchedule.Select(x => new UserSchedulesDTO
        {
            Id = x.Id,
            Shedule = x.Shedule,
            UserId = x.UserId
        }).ToList();
    }
}
