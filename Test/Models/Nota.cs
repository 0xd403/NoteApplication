using System.Text.Json.Serialization;

namespace Test.Models;

public class Nota
{
    [JsonPropertyName("id")]
    public int ID { get; set; }

    [JsonPropertyName("text")]
    public string Name { get; set; }

    [JsonPropertyName("createdDate")]
    public DateTime creation { get; set; }
}
