using Data;
using Microsoft.Extensions.Options;
using Utils;

namespace PhotoRecall.API.Administration;

public class AdministratorService(IOptions<PathsConfig> pathsConfig) : IAdministratorService
{
    private readonly PathsConfig _pathsConfig = pathsConfig.Value;
    
    public void ClearPhotosDir()
    {
        FileUtils.ClearDirectory(FileUtils.GetAbsolutePath(_pathsConfig.PhotosPath));
    }
}