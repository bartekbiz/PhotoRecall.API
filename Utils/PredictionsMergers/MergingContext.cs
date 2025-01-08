using Data.Dtos;

namespace Utils.PredictionsMergers;

public interface IMergeStrategy
{
    List<PredictionDtoMerged> Merge(List<ModelRunResultDto> predictions, object args);
}

public class MergingContext
{
    private IMergeStrategy? MergeStrategy { get; set; }

    public void SetStrategy(IMergeStrategy strategy)
    {
        MergeStrategy = strategy;
    }

    public List<PredictionDtoMerged> Merge(List<ModelRunResultDto> predictions, object args)
    {
        if (MergeStrategy == null) throw new Exception("Set strategy first!");
        
        return MergeStrategy.Merge(predictions, args);
    }
}
