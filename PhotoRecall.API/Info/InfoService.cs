using Data;
using Data.Dtos;
using Data.Enums;
using Microsoft.Extensions.Options;

namespace PhotoRecall.API.Info;

public class InfoService(IOptions<List<ModelRunnerConfig>> yoloRunnersConfig) : IInfoService
{
    private readonly List<ModelRunnerConfig> _modelRunnersConfig = yoloRunnersConfig.Value;

    public List<string> GetAvailableYoloModels()
    {
        return _modelRunnersConfig.SelectMany(s => s.Models).ToList();
    }

    public List<DetectionClassDto> GetAllDetectionClasses()
    {
        var result = new List<DetectionClassDto>();
        
        foreach (DetectionClassEnum yoloClass in (DetectionClassEnum[]) Enum.GetValues(typeof(DetectionClassEnum)))
        {
            result.Add(new DetectionClassDto
            {
                Class = (int)yoloClass,
                Name = Enum.GetName(typeof(DetectionClassEnum), yoloClass) ?? string.Empty
            });
        }

        return result;
    }
}