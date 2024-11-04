namespace Data.Dtos;

public class YoloRunnerResultDto
{
    public YoloRunnerInfoDto YoloRunnerInfo { get; set; }
    public List<PredictionDto>? Predictions { get; set; }
}
