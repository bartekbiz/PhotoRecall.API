using Microsoft.AspNetCore.Http;

namespace Utils;

public static class FileUtils
{
    public static async Task<string> SaveFormFile(string dir, IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            throw new Exception("File is null!");
        }
        
        var newFileName = $"{Guid.NewGuid()}.{file.FileName.Split(".").ToList().Last()}";
        
        var photosPath = Path.Combine(Directory.GetCurrentDirectory(), dir);
        if (!Directory.Exists(photosPath))
        {
            Directory.CreateDirectory(photosPath);
        }
        
        var path = Path.Combine(photosPath, newFileName);
        
        await using var stream = new FileStream(path, FileMode.Create);
        await file.CopyToAsync(stream);

        if (!File.Exists(path))
        {
            throw new Exception("Could not save file!");
        }
        
        return path;
    }

    public static void ClearDirectory(string path)
    {
        
    }
}