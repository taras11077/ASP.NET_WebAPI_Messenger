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
        // отримання сервісу з контексту
        var userStatisticService = context.RequestServices.GetRequiredService<IStatisticService>();
        // отримання шляху з реквеста
        var path = context.Request.Path.Value;
        if (path != null && path.StartsWith("/api/users/search/"))
        {
            // отримання нікнейму з шляху
            var name = path.Replace("/api/users/search/", "");
            
            // оновлення статистики
            userStatisticService.AddUserStatistic(name); 
        }
        
        await _next(context);
    }
}