using CatNoteSchedule.BLL.Models;

namespace CatNoteSchedule.BLL.Abstractions;

public interface ISchedulesService
{
    Task<Dictionary<string, List<string>>> GenerateSchedule(List<ActivityRequestModel> activities, Guid userId, CancellationToken cancellationToken);
    Task<UserSchedulesModel?> GetByUserId(Guid userId, CancellationToken cancellationToken);
    Task<List<UserSchedulesModel>> GetAll(CancellationToken cancellationToken);
    Task<Dictionary<string, List<string>?>> GetById(Guid id, CancellationToken cancellationToken);
}
