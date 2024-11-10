using Data;
using Microsoft.Extensions.Options;
using Utils;

namespace PhotoRecall.API.Administration;

public class AdministratorService(IOptions<PhotosConfig> pathsConfig) : IAdministratorService
{
    private readonly PhotosConfig _photosConfig = pathsConfig.Value;
    
    public void ClearPhotosDir()
    {
        FileUtils.ClearDirectory(FileUtils.GetAbsolutePath(_photosConfig.Path));
    }
}