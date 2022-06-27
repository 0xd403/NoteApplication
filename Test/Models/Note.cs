using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Test.Models;

public class Note
{
    [MaxLength(36)]
    [JsonPropertyName("id")]
    public string Id { get; set; } = Guid.NewGuid().ToString("D");

    [JsonPropertyName("text")]
    public string Text { get; set; } = "";

    [JsonPropertyName("createdDate")]
    public DateTime CreatedDate { get; set; } = DateTime.Now; 
}
