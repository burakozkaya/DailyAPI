namespace DailyAPI.Entity;

public class Log
{
    public int Id { get; set; }
    public string EndPoint { get; set; }
    public string ExceptionMessage { get; set; }
    public string Path { get; set; }
}