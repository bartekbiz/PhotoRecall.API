using PhotoRecall.API.Exceptions;

namespace PhotoRecall.API.Middleware;

public class ErrorHandlingMiddleware(ILogger<ErrorHandlingMiddleware> logger) : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next.Invoke(context);
        }
        catch (NotFoundException e)
        {
            await HandleHttpException(context, e);
        }
        catch (BadRequestException e)
        {
            await HandleHttpException(context, e);
        }
        catch (UnauthorizedException e)
        {
            await HandleHttpException(context, e);
        }
        catch (Exception e)
        {
            logger.LogError(e, e.Message);

            context.Response.StatusCode = 500;
            await context.Response.WriteAsync("Something went wrong :(");
        }
    }

    private async Task HandleHttpException(HttpContext context, HttpExceptionBase e)
    {
        logger.LogError(e, $"Http exception was thrown: Status code: {e.StatusCode}, Message: {e.Message}");
            
        context.Response.StatusCode = e.StatusCode;
        await context.Response.WriteAsync(e.Message);
    }
}