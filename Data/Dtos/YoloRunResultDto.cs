namespace Data.Dtos;

public class YoloRunResultDto
{
    public YoloRunInfoDto YoloRunInfo { get; set; }
    public List<PredictionDto>? Predictions { get; set; }
}
