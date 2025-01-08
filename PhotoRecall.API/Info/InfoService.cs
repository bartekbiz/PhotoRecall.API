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

    public List<YoloClassDto> GetAllYoloClasses()
    {
        var result = new List<YoloClassDto>();
        
        foreach (YoloClassEnum yoloClass in (YoloClassEnum[]) Enum.GetValues(typeof(YoloClassEnum)))
        {
            result.Add(new YoloClassDto
            {
                Class = (int)yoloClass,
                Name = Enum.GetName(typeof(YoloClassEnum), yoloClass) ?? string.Empty
            });
        }

        return result;
    }
}