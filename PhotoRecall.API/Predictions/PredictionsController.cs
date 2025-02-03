using Data.Dtos;
using Microsoft.AspNetCore.Mvc;
using Utils;

namespace PhotoRecall.API.Predictions;

[ApiController]
[Route("api/[controller]")]
public class PredictionsController(IPredictionsService predictionsService) : ControllerBase
{
    [HttpPost]
    [Route("unmerged")]
    public async Task<IActionResult> GetPredictionsAsync(PredictionPropsDto propsDto)
    {
        var predictions = await predictionsService
            .GetPredictionsAsync(propsDto);
        
        return StatusCode(StatusCodes.Status200OK, predictions);
    }
    
    [HttpPost]
    [Route("merged")]
    public async Task<IActionResult> GetMergedPredictionsAsync(PredictionVotingPropsDto propsDto)
    {
        var predictions = await predictionsService
            .GetMergedPredictionsAsync(propsDto);
        
        return StatusCode(StatusCodes.Status200OK, predictions);
    }
    
    [HttpPost]
    [Route("merged-with-counts")]
    public async Task<IActionResult> GetMergedPredictionsWithCountsAsync(PredictionPropsDto propsDto)
    {
        var predictions = await predictionsService
            .GetMergedPredictionsWithCountsAsync(propsDto);
        
        return StatusCode(StatusCodes.Status200OK, predictions);
    }
}