using Data.Dtos;

namespace PhotoRecall.API.Info;

public interface IInfoService
{
    List<string> GetAvailableYoloModels();
    List<YoloClassDto> GetAllYoloClasses();
}