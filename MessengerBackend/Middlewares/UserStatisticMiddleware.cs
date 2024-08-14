using MessengerBackend.Core.Interfaces;

namespace MessengerBackend.Middlewares;

public class UserStatisticMiddleware
{
    private readonly RequestDelegate _next;

    public UserStatisticMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var userStatisticService = context.RequestServices.GetRequiredService<IUserStatisticService>();
        var path = context.Request.Path.Value;
        if (path != null && path.StartsWith("/api/users/search/"))
        {
            var name = path.Replace("/api/users/search/", "");
            
            userStatisticService.AddStatistic(name); // оновлення статистикі
        }
        
        await _next(context);
    }
}