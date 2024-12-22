# ASP.NET Core Custom JWT Authentication(Without Identity)

This project demonstrates how to set up and use JWT (JSON Web Token) authentication in an ASP.NET Core application. The project is compatible with both .NET 7 and .NET 8, with package references provided for each version.

## Features

- Secure authentication using JWT.
- Password hashing with Bcrypt.
- OpenAPI/Swagger documentation.
- Flexible and scalable authentication and authorization mechanisms.

## Requirements

- .NET 7 or .NET 8 SDK installed.

## Installation

1. Clone this repository.
2. Open the project in your preferred IDE or editor.
3. Restore NuGet packages:
   ```bash
   dotnet restore
   ```

## Package References

### For .NET 8
```xml
<PackageReference Include="Bcrypt.Net-Next" Version="4.0.3" />
<PackageReference Include="Microsoft.AspNetCore.Authentication.JwtBearer" Version="8.0.4" />
<PackageReference Include="Microsoft.AspNetCore.OpenApi" Version="8.0.8" />
<PackageReference Include="Swashbuckle.AspNetCore" Version="6.9.0" />
<PackageReference Include="System.IdentityModel.Tokens.Jwt" Version="8.1.2" />
```

### For .NET 7
```xml
<PackageReference Include="Bcrypt.Net-Next" Version="4.0.3" />
<PackageReference Include="Microsoft.AspNetCore.Authentication.JwtBearer" Version="7.0.4" />
<PackageReference Include="Microsoft.AspNetCore.OpenApi" Version="7.0.8" />
<PackageReference Include="Swashbuckle.AspNetCore" Version="6.5.0" />
<PackageReference Include="System.IdentityModel.Tokens.Jwt" Version="6.28.0" />
```

## Configuration

### JWT Utility Implementation

The `JwtUtils` class handles the generation and validation of JSON Web Tokens (JWTs). It is defined in the `njwt.Utils` namespace and implements the `IJwtUtils` interface.

#### Interface: `IJwtUtils`

```csharp
public interface IJwtUtils
{
    string GenerateJwtToken(User user);
    int? ValidateJwtToken(string? token);
}
```

- **`GenerateJwtToken(User user)`**: Creates a JWT token for the given `User`.
- **`ValidateJwtToken(string? token)`**: Validates the given token and returns the user ID if the token is valid; otherwise, returns `null`.

#### Class: `JwtUtils`

##### Constructor

The constructor accepts an `IOptions<AppSetting>` parameter, which must include the JWT secret key.

```csharp
public JwtUtils(IOptions<AppSetting> appSetting)
{
    _appSetting = appSetting.Value;

    if (string.IsNullOrEmpty(_appSetting.Secret))
    {
        throw new Exception("Missing JWT secret");
    }
}
```

- **Dependency**: The `AppSetting` class should include a `Secret` property for the JWT secret key.

##### Method: `GenerateJwtToken(User user)`

This method generates a JWT token for the given user. 

```csharp
public string GenerateJwtToken(User user)
{
    var tokenHandler = new JwtSecurityTokenHandler();
    var key = Encoding.ASCII.GetBytes(_appSetting.Secret!);
    var tokenDescriptor = new SecurityTokenDescriptor
    {
        Subject = new ClaimsIdentity(new[] { new Claim("id", user.Id.ToString()) }),
        Expires = DateTime.UtcNow.AddDays(7),
        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
    };

    var token = tokenHandler.CreateToken(tokenDescriptor);
    return tokenHandler.WriteToken(token);
}
```

- **Input**: `User` object with a unique `Id`.
- **Output**: A signed JWT string valid for 7 days.

##### Method: `ValidateJwtToken(string? token)`

This method validates the given JWT token and extracts the user ID if valid.

```csharp
public int? ValidateJwtToken(string? token)
{
    if (token == null)
        return null;

    var tokenHandler = new JwtSecurityTokenHandler();
    var key = Encoding.ASCII.GetBytes(_appSetting.Secret!);
    try
    {
        tokenHandler.ValidateToken(token, new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = false,
            ValidateAudience = false,
            ClockSkew = TimeSpan.Zero
        }, out SecurityToken validatedToken);

        var jwtToken = (JwtSecurityToken)validatedToken;
        var userId = int.Parse(jwtToken.Claims.First(x => x.Type == "id").Value);

        // Return userId on successful validation
        return userId;
    }
    catch
    {
        return null;
    }
}
```

- **Input**: JWT string (nullable).
- **Output**: User ID as `int?` or `null` if the token is invalid.
- **Validation**: Verifies the signing key, expiration, and claims.

### Custom Authorization Attribute

The `AuthorizeAttribute` class provides role-based and general authorization for controllers and actions. It is defined in the `njwt.Attributes` namespace.

#### Implementation

```csharp
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class AuthorizeAttribute : Attribute, IAuthorizationFilter
{
    private readonly string[] _roles;

    public AuthorizeAttribute(params string[] roles)
    {
        _roles = roles;
    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        // Skip authorization if action is decorated with [AllowAnonymous]
        var allowAnonymous = context.ActionDescriptor.EndpointMetadata.OfType<AllowAnonymousAttribute>().Any();
        if (allowAnonymous)
            return;

        // Authorization
        var user = (User?)context.HttpContext.Items["User"];

        if (user == null)
        {
            // Not logged in or role not authorized
            context.Result = new JsonResult(new { message = "Unauthorized" }) { StatusCode = StatusCodes.Status401Unauthorized };
        }

        // Check if the user has any of the required roles
        if (_roles.Length > 0 && !_roles.Any(role => user?.Role == role))
        {
            // User does not have any of the required roles
            context.Result = new JsonResult(new { message = "Forbidden" }) { StatusCode = StatusCodes.Status403Forbidden };
            return;
        }
    }
}
```

- **Roles**: Accepts an optional list of roles to restrict access to specific user roles.
- **AllowAnonymous**: Skips authorization if the `[AllowAnonymous]` attribute is applied.

### Usage

1. **Register `JwtUtils` and `AuthorizeAttribute` in the Dependency Injection Container**:
   ```csharp
   services.Configure<AppSetting>(Configuration.GetSection("AppSetting"));
   services.AddScoped<IJwtUtils, JwtUtils>();
   ```

2. **Protect Controllers or Actions**:
   ```csharp
   [Authorize("Admin", "User")]
   [ApiController]
   [Route("api/[controller]")]
   public class SampleController : ControllerBase
   {
       [HttpGet("protected")]
       public IActionResult ProtectedEndpoint()
       {
           return Ok(new { message = "This is a protected endpoint" });
       }
   }
   ```

   Use `[Authorize]` without roles for general protection, or specify roles to restrict access.

3. **Allow Anonymous Access**:
   ```csharp
   [AllowAnonymous]
   [HttpGet("public")]
   public IActionResult PublicEndpoint()
   {
       return Ok(new { message = "This is a public endpoint" });
   }
   ```

### Swagger/OpenAPI Integration

Enable Swagger for API documentation:
```csharp
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
```

## Running the Application

1. Build the project:
   ```bash
   dotnet build
   ```

2. Run the application:
   ```bash
   dotnet run
   ```

3. Access the API documentation at `https://localhost:{port}/swagger`.

## License

This project is licensed under the MIT License. See the `LICENSE` file for details.

## Contributions

Contributions are welcome! Feel free to open issues or submit pull requests.

---

Enjoy building your secure ASP.NET Core application with JWT authentication!

