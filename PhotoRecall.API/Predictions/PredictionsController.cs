using Microsoft.AspNetCore.Mvc;

namespace PhotoRecall.API.Predictions;

[ApiController]
[Route("api/[controller]")]
public class PredictionsController(IPredictionsService predictionsService) : ControllerBase
{
    [HttpPost]
    [Route("GetVotedPredictionsAsync")]
    public async Task<IActionResult> GetVotedPredictionsAsync(IFormFile photo)
    {
        var predictions = await predictionsService.GetVotedPredictionsAsync(photo);
        
        return StatusCode(StatusCodes.Status200OK, predictions);
    }
    
    [HttpPost]
    [Route("GetAllPredictionsAsync")]
    public async Task<IActionResult> GetAllPredictionsAsync(IFormFile photo)
    {
        var yoloRunnerResults = await predictionsService.GetAllPredictionsAsync(photo);
        
        return StatusCode(StatusCodes.Status200OK, yoloRunnerResults);
    }
}