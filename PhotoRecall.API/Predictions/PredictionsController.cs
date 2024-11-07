using Data.Dtos;
using Microsoft.AspNetCore.Mvc;
using Utils;

namespace PhotoRecall.API.Predictions;

[ApiController]
[Route("api/[controller]")]
public class PredictionsController(IPredictionsService predictionsService) : ControllerBase
{
    [HttpPost]
    [Route("GetAllPredictionsAsync")]
    public async Task<IActionResult> GetAllPredictionsAsync(IFormFile photo)
    {
        var yoloRunnerResults = await predictionsService.GetAllPredictionsAsync(photo);
        
        return StatusCode(StatusCodes.Status200OK, yoloRunnerResults);
    }
    
    [HttpPost]
    [Route("GetPredictionsAsync")]
    public async Task<IActionResult> GetPredictionsAsync(PredictionPropsDto propsDto)
    {
        var predictions = await predictionsService
            .GetPredictionsAsync(propsDto);
        
        return StatusCode(StatusCodes.Status200OK, predictions);
    }
    
    [HttpPost]
    [Route("GetVotedPredictionsWithCountAsync")]
    public async Task<IActionResult> GetVotedPredictionsWithCountAsync(PredictionPropsDto propsDto)
    {
        var predictions = await predictionsService
            .GetVotedPredictionsWithCountAsync(propsDto);
        
        return StatusCode(StatusCodes.Status200OK, predictions);
    }
}