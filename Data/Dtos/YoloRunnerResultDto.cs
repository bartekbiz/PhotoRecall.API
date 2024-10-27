namespace Data.Dtos;

public class YoloRunnerResultDto
{
    public YoloRunnerInfoDto YoloRunnerInfo { get; set; }
    public List<PredictionDto>? Predictions { get; set; }
    public string? ErrorMessage { get; set; }
}

public class PredictionDto
{
    public string Name { get; set; }
    public int Class { get; set; }
    public double Confidence { get; set; }
    public BoxDto? Box { get; set; }
}

public class BoxDto
{
    public double X1 { get; set; }
    public double Y1 { get; set; }
    public double X2 { get; set; }
    public double Y2 { get; set; }
}