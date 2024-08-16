using MessengerBackend.Core.Interfaces;

namespace MessengerBackend.Middlewares;

public class StatisticMiddleware
{
    private readonly RequestDelegate _next;

    public StatisticMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var userStatisticService = context.RequestServices.GetRequiredService<IStatisticService>();
        var path = context.Request.Path.Value;
        if (path != null && path.StartsWith("/api/users/search/"))
        {
            var name = path.Replace("/api/users/search/", "");
            
            userStatisticService.AddUserStatistic(name); // оновлення статистикі
        }
        
        await _next(context);
    }
}