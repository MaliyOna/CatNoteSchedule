using CatNoteSchedule.DAL.Abstractions;
using CatNoteSchedule.DAL.Models;
using Microsoft.EntityFrameworkCore;
using Z.EntityFramework.Plus;

namespace CatNoteSchedule.DAL.Repositories;

public class UserSchedulesRepository : IUserSchedulesRepository
{
    private readonly ApplicationDbContext _applicationDbContext;

    public UserSchedulesRepository(ApplicationDbContext applicationDbContext)
    {
        _applicationDbContext = applicationDbContext;
    }

    public async Task Add(UserSсhedules userShedules, CancellationToken cancellationToken)
    {
        var contain = await GetByUserId(userShedules.UserId, cancellationToken);

        if (contain != null)
        {
            await Delete(contain.UserId, cancellationToken);
        }

        await _applicationDbContext.UserShedules.AddAsync(userShedules, cancellationToken);
        _applicationDbContext.SaveChanges();
    }

    public async Task<List<UserSсhedules>> GetAll(CancellationToken cancellationToken)
    {
        return await _applicationDbContext.UserShedules.ToListAsync(cancellationToken);
    }

    public async Task<UserSсhedules?> GetById(Guid id, CancellationToken cancellationToken)
    {
        return await _applicationDbContext.UserShedules.Where(x => x.Id == id).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<UserSсhedules?> GetByUserId(Guid userId, CancellationToken cancellationToken)
    {
        return await _applicationDbContext.UserShedules.Where(x => x.UserId == userId).FirstOrDefaultAsync(cancellationToken);
    }

    private async Task Delete(Guid UserId, CancellationToken cancellationToken)
    {
        await _applicationDbContext.UserShedules.Where(x => x.UserId == UserId).DeleteAsync(cancellationToken);
        await _applicationDbContext.SaveChangesAsync();
    }
}
