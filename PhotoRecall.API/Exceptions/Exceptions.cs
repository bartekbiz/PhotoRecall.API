namespace PhotoRecall.API.Exceptions;

public class HttpExceptionBase(string message) : Exception(message)
{
    public int StatusCode { get; set; }
}

public class BadRequestException(string message) : HttpExceptionBase(message)
{
    public int StatusCode { get; set; } = 400;
}

public class UnauthorizedException(string message) : HttpExceptionBase(message)
{
    public int StatusCode { get; set; } = 401;
}

public class NotFoundException(string message) : HttpExceptionBase(message)
{
    public int StatusCode { get; set; } = 404;
}