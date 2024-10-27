using Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace PhotoRecall.API.Predictions;

[ApiController]
[Route("api/[controller]")]
public class PredictionsController : ControllerBase
{
    private readonly IPredictionsService _predictionsService;
    private readonly IOptions<List<YoloRunnerConfig>> _yoloRunnersConfig;

    public PredictionsController(IPredictionsService predictionsService, 
        IOptions<List<YoloRunnerConfig>> yoloRunnersConfig)
    {
        _predictionsService = predictionsService;
        _yoloRunnersConfig = yoloRunnersConfig;
    }
    
    [HttpPost]
    [Route("GetVotedPredictionsAsync")]
    public async Task<IActionResult> GetVotedPredictionsAsync()
    {
        var listPredictionDto = await _predictionsService.GetVotedPredictionsAsync(_yoloRunnersConfig.Value);
        
        return StatusCode(StatusCodes.Status200OK, listPredictionDto);
    }
    
    [HttpPost]
    [Route("GetAllPredictionsAsync")]
    public async Task<IActionResult> GetAllPredictionsAsync()
    {
        var listPredictionDto = await _predictionsService.GetAllPredictionsAsync(_yoloRunnersConfig.Value);
        
        return StatusCode(StatusCodes.Status200OK, listPredictionDto);
    }
}