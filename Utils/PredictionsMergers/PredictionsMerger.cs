using Data.Dtos;

namespace Utils.PredictionsMergers;

public abstract class PredictionsMerger<TResult> where TResult : PredictionDtoBase
{
    public abstract List<TResult> Merge();
}