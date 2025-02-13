using Microsoft.AspNetCore.Http;

namespace Data.Dtos;

public class PredictionPropsDto
{
    public IFormFile Photo { get; set; }
    public string Models { get; set; } = string.Empty;
}

public class PredictionVotingPropsDto : PredictionPropsDto
{
    /// <summary>
    /// A value ranging from 0 to 100, representing the percentage of the models
    /// that must agree for a prediction to appear in the results.
    /// </summary>
    public double AgreeRatio { get; set; } = 0;
}

