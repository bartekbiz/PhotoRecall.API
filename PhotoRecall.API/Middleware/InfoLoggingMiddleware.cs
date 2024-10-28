namespace PhotoRecall.API.Middleware;

public class InfoLoggingMiddleware : IMiddleware
{
    private readonly ILogger<InfoLoggingMiddleware> _logger;

    public InfoLoggingMiddleware(ILogger<InfoLoggingMiddleware> logger)
    {
        _logger = logger;
    }
    
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        _logger.LogInformation($"Endpoint invoked: " +
                               $"path: {context.Request.Path}, " +
                               $"remote ip: {context.Connection.RemoteIpAddress}", context);
        await next.Invoke(context);
    }
}