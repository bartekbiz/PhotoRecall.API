using Microsoft.AspNetCore.Http;

namespace Utils;

public static class UriUtils
{
    public static string CreateUri(HttpRequest request, string path)
    {
        return CreateUri(
            scheme: request.Scheme, 
            host: request.Host.ToString(), 
            path: path);
    }
    
    public static string CreateUri(string scheme, string host)
    {
        return $"{scheme}://{host}";
    }

    public static string CreateUri(string scheme, string host, string path)
    {
        return $"{scheme}://{host}/{path}";
    }
}