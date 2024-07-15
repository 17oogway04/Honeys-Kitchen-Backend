using System.Security.Claims;
using Honeys_Kitchen_backend.Models;
using Honeys_Kitchen_backend.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Honeys_Kitchen_backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly ILogger<UserController> _logger;
    private readonly IUserRepository _userRepository;

    public UserController(ILogger<UserController> logger, IUserRepository repository)
    {
        _logger = logger;
        _userRepository = repository;
    }

    [HttpPost]
    [Route("register")]
    public ActionResult CreateUser(User user)
    {
        if(user == null || !ModelState.IsValid)
        {
            return BadRequest();
        }

        _userRepository.CreateUser(user);
        return NoContent();
    }

    [HttpGet]
    [Route("login")]
    public ActionResult<string> SignIn(string email, string password)
    {
        if(string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            return BadRequest();
        }

        var token = _userRepository.SignIn(email, password);

        if(string.IsNullOrWhiteSpace(token))
        {
            return Unauthorized();
        }

        return Ok(token);
    }

    [HttpGet]
    [Route("current")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public ActionResult GetCurrentUser()
    {
        bool isLoggedIn = User.Identity!.IsAuthenticated;
        if(!isLoggedIn)
        {
            return NotFound();
        }

        var id = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
        User currentUser = _userRepository.GetUserById(id);

        return Ok(currentUser);
    }

}