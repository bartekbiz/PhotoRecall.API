using Microsoft.AspNetCore.Http;

namespace Utils;

public static class FileUtils
{
    private const string StaticContentDir = "wwwroot";

    public struct HostedFile
    {
        public string Path { get; init; }
        public string Uri { get; init; }
    }
    
    public static async Task<HostedFile> SaveAndHostFile(string path, string url, IFormFile file)
    {
        var pathToFile = await SaveFile(GetAbsolutePath(path), file);
        
        return new HostedFile
        {
            Path = pathToFile,
            Uri = UriUtils.CreateUri(url, GetRelativeToStaticContentPath(pathToFile))
        };
    }
    
    public static async Task<string> SaveFile(string path, IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            throw new Exception("File is null!");
        }
        
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        var newFileName = CreateRandomFileName(file.FileName);
        var pathToFile = Path.Combine(path, newFileName);

        await using var stream = new FileStream(pathToFile, FileMode.Create);
        await file.CopyToAsync(stream);
        if (!File.Exists(pathToFile))
        {
            throw new Exception("Could not save file!");
        }

        return pathToFile;
    }
    
    public static string CreateRandomFileName(string fileName)
    {
        return $"{Guid.NewGuid()}.{fileName.Split(".").Last()}";
    }
    
    public static string GetAbsolutePath(string relativePath)
    {
        return Path.Combine(Directory.GetCurrentDirectory(), StaticContentDir, relativePath);
    }
    
    public static string GetRelativeToStaticContentPath(string absolutePath)
    {
        return Path.GetRelativePath(StaticContentDir, absolutePath);
    }
    
    public static void DeleteFile(string pathToFile)
    {
        if (!File.Exists(pathToFile))
        {
            return;
        }
        
        File.Delete(pathToFile);
    }
    
    public static void ClearDirectory(string path)
    {
        if (!Directory.Exists(path))
        {
            return;
        }
        
        Directory.Delete(path, recursive: true);
        Directory.CreateDirectory(path);
    }
}