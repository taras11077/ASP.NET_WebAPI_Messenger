namespace MessengerBackend.Middlewares;

public static class MiddlewareExtensions
{
    public static IApplicationBuilder UseInfo(this IApplicationBuilder app)
    {
        return app.UseMiddleware<InfoMiddleware>();
    }
    
    public static IApplicationBuilder UseScreening(this IApplicationBuilder app)
    {
        return app.UseMiddleware<MessageScreeningMiddleware>();
    }
    
}