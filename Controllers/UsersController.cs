namespace njwt.Controllers;

using Microsoft.AspNetCore.Mvc;
using njwt.Services;
using njwt.Models;
using njwt.Attributes;

[ApiController]

[Route("[controller]")]
public class UsersController : ControllerBase
{
    private IUserService _userService;

    public UsersController(IUserService userService)
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

    [Authorize("Admin")]
    [HttpGet]
    public IActionResult GetAll()
    {
        var users = _userService.GetAll();
        return Ok(users);
    }
}
