using Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Utils;

namespace PhotoRecall.API.Administration;

[ApiController]
[Route("api/[controller]")]
public class AdministrationController(IOptions<PathsConfig> pathsConfig) : ControllerBase
{
    private readonly PathsConfig _pathsConfig = pathsConfig.Value;

    [HttpDelete]
    [Route("ClearPhotosDir")]
    public IActionResult ClearPhotosDir()
    {
        FileUtils.ClearDirectory(FileUtils.GetAbsolutePath(_pathsConfig.PhotosPath));

        return StatusCode(StatusCodes.Status204NoContent);
    }
}