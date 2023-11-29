using Microsoft.AspNetCore.Identity;

namespace DailyAPI.Entity;

public class AppUser : IdentityUser
{
    public string? Name { get; set; }
    public string? Surname { get; set; }
    public DateTime? BirthDate { get; set; }

    public decimal? Balance { get; set; }
    public List<Daily> Dailies { get; set; }
}