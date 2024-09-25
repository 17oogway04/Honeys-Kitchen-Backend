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
    private readonly IWebHostEnvironment _env;


    public UserController(ILogger<UserController> logger, IUserRepository repository, IWebHostEnvironment env)
    {
        _logger = logger;
        _userRepository = repository;
        _env = env;
    }

    [HttpPost]
    [Route("register")]
    public ActionResult CreateUser(User user)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        _userRepository.CreateUser(user);
        return NoContent();
    }

    [HttpGet]
    [Route("login")]
    public ActionResult<string> SignIn(string email, string password)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            return BadRequest();
        }

        var token = _userRepository.SignIn(email, password);

        if (string.IsNullOrWhiteSpace(token))
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
        if (!isLoggedIn)
        {
            return NotFound();
        }

        var id = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
        User currentUser = _userRepository.GetUserById(id);

        return Ok(currentUser);
    }

    [HttpPost]
    [Route("upload-profile-picture")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<IActionResult> UploadProfilePicture(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest("No file uploaded.");
        }
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
        var user = _userRepository.GetUserById(userId);
        if (user == null)
        {
            return NotFound("User not found.");
        }

        var uploads = Path.Combine(_env.WebRootPath, "profile-pictures");
        if (!Directory.Exists(uploads))
        {
            Directory.CreateDirectory(uploads);
        }
        var filePath = Path.Combine(uploads, file.FileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }


        if (user != null)
        {
            user.ProfilePicture = $"/profile-pictures/{file.FileName}";
            _userRepository.UpdateUser(user);
        }
        return Ok(new { FilePath = $"/profile-pictures/{file.FileName}" });

    }


    [HttpGet]
    [Route("by-username/{username}")]
    public async Task<ActionResult<User>> GetUserByUsername(string username)
    {
        var name = _userRepository.GetUserByUsername(username);
        if(name == null){
            return NotFound();
        }

        return Ok(await name);
    }

}