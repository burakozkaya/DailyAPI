using DailyAPI.Context;
using DailyAPI.Entity;

namespace DailyAPI.Middleware;

public class ExceptionHandling
{
    private readonly RequestDelegate _next;

    public ExceptionHandling(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        var dbContext = context.RequestServices.GetRequiredService<AppDbContext>();
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            Log log = new Log
            {
                EndPoint = context.Request.Method,
                ExceptionMessage = ex.Message,
                Path = context.Request.Path
            };
            dbContext.Logs.Add(log);
            await dbContext.SaveChangesAsync();
        }
    }
}