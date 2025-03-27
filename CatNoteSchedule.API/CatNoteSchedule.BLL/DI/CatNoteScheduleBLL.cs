using CatNoteSchedule.BLL.Abstractions;
using CatNoteSchedule.BLL.Services;
using CatNoteSchedule.DAL.DI;
using Microsoft.Extensions.DependencyInjection;

namespace CatNoteSchedule.BLL.DI;
public static class CatNoteScheduleBLL
{
    public static void AddBLLServices(this IServiceCollection services, string? connectionString)
    {
        services.AddScoped<ISchedulesService, SchedulesService>();
        services.AddDalServisces(connectionString);
    }
}
