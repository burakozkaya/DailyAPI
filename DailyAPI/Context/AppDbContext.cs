using DailyAPI.Entity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DailyAPI.Context;

public class AppDbContext : IdentityDbContext<AppUser>
{
    public DbSet<Daily> Dailies { get; set; }
    public DbSet<Log> Logs { get; set; }
    public AppDbContext(DbContextOptions options) : base(options)
    {

    }
}