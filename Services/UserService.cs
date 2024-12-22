using njwt.Models;
using njwt.Utils;

namespace njwt.Services;


public interface IUserService
{
    AuthenticateResponse? Authenticate(AuthenticateRequest model);
    IEnumerable<User> GetAll();
    User? GetUserById(int id);
}
public class UserService : IUserService
{
    private List<User> _users = new List<User>{
      new User{Id=1,Name="John Doe",Username="john123",Password="$2a$12$3idBuJES8UHPEwrSqnhhUeCCRp1JYWS6ABIwJf26goMGxXBw/IQiq", Role="Admin"},
      new User{Id=2,Name="Sam Altman",Username="altman123",Password="$2a$12$3idBuJES8UHPEwrSqnhhUeCCRp1JYWS6ABIwJf26goMGxXBw/IQiq", Role="User"},
    };

    private readonly IJwtUtils _jwtUtils;
    private readonly IHashUtils _hashUtils;

    public UserService(IJwtUtils jwtUtils, IHashUtils hashUtils)
    {
        _jwtUtils = jwtUtils;
        _hashUtils = hashUtils;
    }




    public AuthenticateResponse? Authenticate(AuthenticateRequest model)
    {
        var user = _users.SingleOrDefault(x =>
            x.Username == model.Username && _hashUtils.VerifyHash(model.Password!, x.Password!));

        // return null if user not found 
        if (user == null) return null;

        var token = _jwtUtils.GenerateJwtToken(user);

        return new AuthenticateResponse(user, token);
    }

    public IEnumerable<User> GetAll()
    {
        return _users;
    }

    public User? GetUserById(int id)
    {
        return this._users.FirstOrDefault(x => x.Id == id);
    }
}
