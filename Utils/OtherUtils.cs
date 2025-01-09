using Data.Enums;
using Newtonsoft.Json;

namespace Utils;

public static class OtherUtils
{
    /// <summary>
    /// Tries to convert json string to string list.
    /// </summary>
    /// <returns>List of strings if conversion went ok, otherwise returns null.</returns>
    public static List<string>? TryConvertJsonStringToList(string jsonString)
    {
        List<string>? result = null;

        try
        {
            result = JsonConvert.DeserializeObject<List<string>>(jsonString);
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error converting json string to list, description: {e.Message}");
        }

        return result;
    }
    
    public static DetectionClassEnum? TryMapPhraseToDetectionClass(string phrase)
    {
        return TryMapPhraseToEnum<DetectionClassEnum>(phrase);
    }

    /// <summary>
    /// Tries to map string phrase to enum type.
    /// </summary>
    /// <typeparam name="T">Is a not nullable value type, Enum</typeparam>
    /// <returns></returns>
    public static T? TryMapPhraseToEnum<T>(string phrase) where T : struct, Enum
    {
        phrase = phrase.Trim().Replace(" ", "").ToLower();
        
        if (Enum.TryParse(phrase, ignoreCase: true, out T value))
        {
            return value;
        }
        
        return null;
    }
}