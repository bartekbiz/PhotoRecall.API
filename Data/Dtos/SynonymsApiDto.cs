namespace Data.Dtos;

public class SynonymsApiDto
{
    public string Word { get; set; }
    public List<string> Synonyms { get; set; }
    public List<string> Antonyms { get; set; }
}