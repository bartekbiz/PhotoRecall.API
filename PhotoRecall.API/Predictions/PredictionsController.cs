using Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace PhotoRecall.API.Predictions;

[ApiController]
[Route("api/[controller]")]
public class PredictionsController : ControllerBase
{
    private readonly IPredictionsService _predictionsService;

    public PredictionsController(IPredictionsService predictionsService)
    {
        _predictionsService = predictionsService;
    }
    
    [HttpPost]
    [Route("GetVotedPredictionsAsync")]
    public async Task<IActionResult> GetVotedPredictionsAsync()
    {
        var predictions = await _predictionsService.GetVotedPredictionsAsync();
        
        return StatusCode(StatusCodes.Status200OK, predictions);
    }
    
    [HttpPost]
    [Route("GetAllPredictionsAsync")]
    public async Task<IActionResult> GetAllPredictionsAsync()
    {
        var yoloRunnerResults = await _predictionsService.GetAllPredictionsAsync();
        
        return StatusCode(StatusCodes.Status200OK, yoloRunnerResults);
    }
}