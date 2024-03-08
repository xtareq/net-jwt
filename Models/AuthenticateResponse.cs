
namespace njwt.Models;


public class AuthenticateResponse
{

    public int Id { get; set; }

    public string? Name { get; set; }
    public string? Username { get; set; }
    public string? Role { get; set; }

    public string Token { get; set; }

    public AuthenticateResponse(User user, string token)
    {
        Id = user.Id;
        Name = user.Name;
        Username = user.Username;
        Token = token;
    }
}
