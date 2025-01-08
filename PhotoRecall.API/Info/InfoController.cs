using Microsoft.AspNetCore.Mvc;

namespace PhotoRecall.API.Info;

[ApiController]
[Route("api/[controller]")]
public class InfoController(IInfoService infoService) : ControllerBase
{
    [HttpGet]
    [Route("GetAvailableYoloModels")]
    public IActionResult GetAvailableYoloModels()
    {
        var yoloModels = infoService.GetAvailableYoloModels();
        
        return StatusCode(StatusCodes.Status200OK, yoloModels);
    }
    
    [HttpGet]
    [Route("GetAllDetectionClasses")]
    public IActionResult GetAllDetectionClasses()
    {
        var yoloClasses = infoService.GetAllDetectionClasses();
        
        return StatusCode(StatusCodes.Status200OK, yoloClasses);
    }
}