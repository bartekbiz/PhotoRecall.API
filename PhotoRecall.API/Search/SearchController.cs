using Microsoft.AspNetCore.Mvc;

namespace PhotoRecall.API.Search;

[ApiController]
[Route("api/[controller]")]
public class SearchController(ISearchService searchService) : ControllerBase
{
    [HttpGet]
    [Route("GetYoloClassesAsync")]
    public async Task<IActionResult> GetYoloClassesAsync(string phrase)
    {
        var classes = await searchService.GetYoloClassesAsync(phrase);
        
        return StatusCode(StatusCodes.Status200OK, classes);
    }
}