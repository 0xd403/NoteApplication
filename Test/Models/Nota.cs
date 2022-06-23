using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Test.Models;

public class Nota
{
    [MaxLength(36)]
    [JsonPropertyName("id")]
    public string ID { get; set; } = Guid.NewGuid().ToString("D");

    [JsonPropertyName("text")]
    public string Name { get; set; } = "";

    [JsonPropertyName("createdDate")]
    public DateTime creation { get; set; }
}
