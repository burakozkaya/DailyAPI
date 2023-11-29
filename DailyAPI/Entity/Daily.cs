namespace DailyAPI.Entity;

public class Daily
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public DateTime AddedDate { get; set; }
    public string AddedIP { get; set; }

    public string AppUserId { get; set; }
    //Nav Property
    public AppUser AppUser { get; set; }

}