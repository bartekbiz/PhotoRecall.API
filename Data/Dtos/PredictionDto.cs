namespace Data.Dtos;

public class PredictionDtoBase
{
    public string Name { get; set; }
}

public class PredictionDto : PredictionDtoBase
{
    public int Class { get; set; }
    public double? Confidence { get; set; }
    public BoxDto? Box { get; set; }
}

public class BoxDto
{
    public double X1 { get; set; }
    public double Y1 { get; set; }
    public double X2 { get; set; }
    public double Y2 { get; set; }
}

public class PredictionWithCountDto : PredictionDtoBase
{
    public int Count { get; set; }
}

