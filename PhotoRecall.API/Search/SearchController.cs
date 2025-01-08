using Microsoft.AspNetCore.Mvc;

namespace PhotoRecall.API.Search;

[ApiController]
[Route("api/[controller]")]
public class SearchController(ISearchService searchService) : ControllerBase
{
    [HttpGet]
    [Route("GetDetectionClassesAsync")]
    public async Task<IActionResult> GetDetectionClassesAsync(string phrase)
    {
        var classes = await searchService.GetDetectionClassesAsync(phrase);
        
        return StatusCode(StatusCodes.Status200OK, classes);
    }
}