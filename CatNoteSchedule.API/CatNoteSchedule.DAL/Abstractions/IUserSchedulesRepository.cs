using CatNoteSchedule.DAL.Models;

namespace CatNoteSchedule.DAL.Abstractions;
public interface IUserSchedulesRepository
{
    Task Add(UserSсhedules userShedules, CancellationToken cancellationToken);
    Task<List<UserSсhedules>> GetAll(CancellationToken cancellationToken);
    Task<UserSсhedules?> GetById(Guid id, CancellationToken cancellationToken);
    Task<UserSсhedules?> GetByUserId(Guid userId, CancellationToken cancellationToken);
}
