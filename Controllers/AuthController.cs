namespace njwt.Controllers;

using Microsoft.AspNetCore.Mvc;
using njwt.Services;
using njwt.Models;
using njwt.Attributes;

[ApiController]
[Authorize]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    private IUserService _userService;

    public AuthController(IUserService userService)
    {
        _userService = userService;
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public IActionResult Login(AuthenticateRequest model)
    {
        var response = _userService.Authenticate(model);
        if (response == null)
            return BadRequest(new { message = "Invalid username or password" });

        return Ok(response);
    }


    [HttpGet("account")]
    public IActionResult GetProfile()
    {
        var user = HttpContext.Items["User"] as User;

        return Ok(user);
    }
}
