namespace Data.Dtos;

public class ModelRunResultDto
{
    public ModelRunInfoDto ModelRunInfo { get; set; }
    public List<PredictionDto>? Predictions { get; set; }
}
