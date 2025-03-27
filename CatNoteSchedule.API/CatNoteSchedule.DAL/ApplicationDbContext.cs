using CatNoteSchedule.DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace CatNoteSchedule.DAL;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<UserSсhedules> UserShedules { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
    }
}
