namespace MessengerBackend.Middlewares;

public class InfoMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<InfoMiddleware> _logger;

    public InfoMiddleware(RequestDelegate next, ILogger<InfoMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext ctx)
    {
        using var sr = new StreamReader(ctx.Request.Body);
        string info = $"Path: {ctx.Request.Path}{Environment.NewLine}Body: {await sr.ReadToEndAsync()}";
        
        await _next.Invoke(ctx);
        _logger.LogInformation(info);
    }
}

