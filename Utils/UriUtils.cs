using Microsoft.AspNetCore.Http;

namespace Utils;

public static class UriUtils
{
    #region CreateUri
    
    public static string CreateUri(HttpRequest request, string path)
    {
        return CreateUri(
            scheme: request.Scheme, 
            host: request.Host.ToString(), 
            path: path);
    }
    
    public static string CreateUri(string scheme, string host, string path)
    {
        string url = CreateUrl(scheme, host);
        return CreateUri(url, path);
    }
    
    public static string CreateUri(string url, string path)
    {
        return new Uri(new Uri(url), path).ToString();
    }

    #endregion
    
    #region CreateUrl

    public static string CreateUrl(HttpRequest request)
    {
        return CreateUrl(
            scheme: request.Scheme, 
            host: request.Host.ToString());
    }
    
    public static string CreateUrl(string scheme, string host)
    {
        return $"{scheme}://{host}";
    }

    #endregion
}