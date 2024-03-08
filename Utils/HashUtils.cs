namespace njwt.Utils;
using BCrypt.Net;

public interface IHashUtils
{
    string MakeHash(string password);
    bool VerifyHash(string password, string hash);
}


public class HashUtils : IHashUtils
{

    public string MakeHash(string password)
    {
        // generate salt 
        string salt = BCrypt.GenerateSalt();

        // now hash password 

        string hashPassword = BCrypt.HashPassword(password, salt);
        return hashPassword;
    }

    public bool VerifyHash(string password, string hash)
    {
        return BCrypt.Verify(password, hash);
    }
}
