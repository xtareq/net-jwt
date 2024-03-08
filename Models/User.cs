namespace njwt.Models;

using System.Text.Json.Serialization;

public class User
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Username { get; set; }

    public string? Role {get; set;}

    [JsonIgnore]
    public string? Password { get; set; }
}
