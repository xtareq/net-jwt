using njwt.Services;
using njwt.Utils;

namespace njwt.Middlewares;


public class JwtMiddleware
{
    private readonly RequestDelegate _next;

    public JwtMiddleware(RequestDelegate next)
    {
        _next = next;
    }


    public async Task Invoke(HttpContext context, IUserService userService, IJwtUtils jwtUtils)
    {

        var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
        var userId = jwtUtils.ValidateJwtToken(token);
        if (userId != null)
        {
            // attch user to context on successful jwt validtion 
            context.Items["User"] = userService.GetUserById(userId.Value);
        }
        await _next(context);
    }

}
