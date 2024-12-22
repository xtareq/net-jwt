# ASP.NET Core JWT Authentication Example

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

### JWT Authentication Setup

1. Add the required services in `Program.cs` or `Startup.cs`:
   ```csharp
   builder.Services.AddAuthentication(options =>
   {
       options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
       options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
   })
   .AddJwtBearer(options =>
   {
       options.TokenValidationParameters = new TokenValidationParameters
       {
           ValidateIssuer = true,
           ValidateAudience = true,
           ValidateLifetime = true,
           ValidateIssuerSigningKey = true,
           ValidIssuer = "your-issuer",
           ValidAudience = "your-audience",
           IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("your-secret-key"))
       };
   });
   ```

2. Configure middleware in the pipeline:
   ```csharp
   app.UseAuthentication();
   app.UseAuthorization();
   ```

### Password Hashing with Bcrypt

Use the `Bcrypt.Net-Next` package for secure password hashing and verification:
```csharp
string hashedPassword = BCrypt.Net.BCrypt.HashPassword("your-password");
bool isPasswordValid = BCrypt.Net.BCrypt.Verify("your-password", hashedPassword);
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

