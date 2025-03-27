using CatNoteSchedule.DAL.Abstractions;
using CatNoteSchedule.DAL.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CatNoteSchedule.DAL.DI;

public static class CatNoteScheduleDAL
{
    public static void AddDalServisces(this IServiceCollection services, string? connectionString)
    {
        services.AddScoped<IUserSchedulesRepository, UserSchedulesRepository>();
        services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));
    }
}
