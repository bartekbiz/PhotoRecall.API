using Newtonsoft.Json;

namespace Utils;

public static class OtherUtils
{
    public static List<string>? ConvertJsonStringToList(string jsonString)
    {
        List<string>? result = null;

        try
        {
            result = JsonConvert.DeserializeObject<List<string>>(jsonString);
        }
        catch
        { }

        return result;
    }
}