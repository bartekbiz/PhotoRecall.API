using Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Utils;

namespace PhotoRecall.API.Administration;

[ApiController]
[Route("api/[controller]")]
public class AdministrationController(IAdministratorService administratorService) : ControllerBase
{
    [HttpDelete]
    [Route("ClearPhotosDir")]
    public IActionResult ClearPhotosDir()
    {
        administratorService.ClearPhotosDir();

        return StatusCode(StatusCodes.Status204NoContent);
    }
}