using Microsoft.AspNetCore.Http;

namespace Utils;

public static class FileUtils
{
    public static async Task<string> SaveFormFile(string path, IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            throw new Exception("File is null!");
        }

        var absolutePath = GetAbsolutePath(path);
        if (!Directory.Exists(absolutePath))
        {
            Directory.CreateDirectory(absolutePath);
        }

        var newFileName = CreateRandomFileName(file.FileName);
        
        var pathToFile = Path.Combine(absolutePath, newFileName);
        
        await using var stream = new FileStream(pathToFile, FileMode.Create);
        await file.CopyToAsync(stream);

        if (!File.Exists(pathToFile))
        {
            throw new Exception("Could not save file!");
        }
        
        return pathToFile;
    }

    public static void DeleteFile(string path)
    {
        var absolutePath = GetAbsolutePath(path);
        if (!File.Exists(absolutePath))
        {
            return;
        }
        
        File.Delete(absolutePath);
    }
    
    public static void ClearDirectory(string path)
    {
        var absolutePath = GetAbsolutePath(path);
        if (!Directory.Exists(absolutePath))
        {
            return;
        }
        
        Directory.Delete(absolutePath, recursive: true);
        Directory.CreateDirectory(absolutePath);
    }
    
    private static string GetAbsolutePath(string path)
    {
        return Path.Combine(Directory.GetCurrentDirectory(), path);
    }
    
    private static string CreateRandomFileName(string fileName)
    {
        return $"{Guid.NewGuid()}.{fileName.Split(".").ToList().Last()}";
    }
}