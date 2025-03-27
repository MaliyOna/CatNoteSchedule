using CatNoteSchedule.DAL.Abstractions;
using CatNoteSchedule.DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace CatNoteSchedule.DAL.Repositories;

public class UserSchedulesRepository : IUserSchedulesRepository
{
    private readonly DbSet<UserSсhedules> _dbSet;

    public UserSchedulesRepository(ApplicationDbContext applicationDbContext)
    {
        _dbSet = applicationDbContext.Set<UserSсhedules>();
    }

    public async Task Add(UserSсhedules userShedules, CancellationToken cancellationToken)
    {
        await _dbSet.AddAsync(userShedules, cancellationToken);
    }

    public async Task<List<UserSсhedules>> GetAll(CancellationToken cancellationToken)
    {
        return await _dbSet.ToListAsync(cancellationToken);
    }

    public async Task<UserSсhedules?> GetById(Guid id, CancellationToken cancellationToken)
    {
        return await _dbSet.Where(x => x.Id == id).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<UserSсhedules?> GetByUserId(Guid userId, CancellationToken cancellationToken)
    {
        return await _dbSet.Where(x => x.UserId == userId).FirstOrDefaultAsync(cancellationToken);
    }
}
