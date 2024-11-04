namespace PhotoRecall.API.Exceptions;

public class HttpExceptionBase(string message) : Exception(message)
{
    public int StatusCode { get; protected init; }
}

public class BadRequestException : HttpExceptionBase
{
    public BadRequestException(string message) : base(message)
    {
        StatusCode = 400;
    }
}

public class UnauthorizedException : HttpExceptionBase
{
    public UnauthorizedException(string message) : base(message)
    {
        StatusCode = 401;
    }
}

public class NotFoundException : HttpExceptionBase
{
    public NotFoundException(string message) : base(message)
    {
        StatusCode = 404;
    }
}