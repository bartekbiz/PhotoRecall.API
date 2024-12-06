using Microsoft.AspNetCore.Http;

namespace Data.Dtos;

public class PredictionPropsDto
{
    public IFormFile Photo { get; set; }
    public string YoloModels { get; set; } = string.Empty;
    public double AgreeRatio { get; set; } = 0;
}