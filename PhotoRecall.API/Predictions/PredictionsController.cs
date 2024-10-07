using Microsoft.AspNetCore.Mvc;

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
    
    [HttpGet]
    [Route("GetPredictions")]
    public async Task<IActionResult> GetPredictionsAsync()
    {
        var listPredictionDto = await _predictionsService.GetPredictionsAsync();
        
        return StatusCode(StatusCodes.Status200OK, listPredictionDto);
    }
}