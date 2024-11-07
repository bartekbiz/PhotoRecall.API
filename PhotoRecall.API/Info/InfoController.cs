using Microsoft.AspNetCore.Mvc;

namespace PhotoRecall.API.Info;

[ApiController]
[Route("api/[controller]")]
public class InfoController(IInfoService infoService) : ControllerBase
{
    [HttpGet]
    [Route("GetAllYoloModelsAsync")]
    public IActionResult GetAvailableYoloModelsAsync()
    {
        var yoloModels = infoService.GetAvailableYoloModelsAsync();
        
        return StatusCode(StatusCodes.Status200OK, yoloModels);
    }
}