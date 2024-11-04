using Microsoft.AspNetCore.Mvc;
using Utils;

namespace PhotoRecall.API.Predictions;

[ApiController]
[Route("api/[controller]")]
public class PredictionsController(IPredictionsService predictionsService) : ControllerBase
{
    [HttpPost]
    [Route("GetVotedPredictionsWithCountAsync")]
    public async Task<IActionResult> GetVotedPredictionsWithCountAsync(IFormFile photo)
    {
        var predictions = await predictionsService
            .GetVotedPredictionsWithCountAsync(photo);
        
        return StatusCode(StatusCodes.Status200OK, predictions);
    }
    
    [HttpPost]
    [Route("GetAllPredictionsAsync")]
    public async Task<IActionResult> GetAllPredictionsAsync(IFormFile photo)
    {
        var yoloRunnerResults = await predictionsService
            .GetAllPredictionsAsync(photo);
        
        return StatusCode(StatusCodes.Status200OK, yoloRunnerResults);
    }
}