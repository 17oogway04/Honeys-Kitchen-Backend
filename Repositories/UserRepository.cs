using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Honeys_Kitchen_backend.Migrations;
using Models = Honeys_Kitchen_backend.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using bcrypt = BCrypt.Net.BCrypt;


namespace Honeys_Kitchen_backend.Repositories;

public class UserRepository : IUserRepository
{
    private static HoneysKitchenDbContext _context;
    private static IConfiguration? _config;

    public UserRepository(HoneysKitchenDbContext context, IConfiguration config)
    {
        _context = context;
        _config = config;
    }

    private string BuildToken(Models.AppUser user)
    {
        var secret = _config?.GetValue<string>("TokenSecret");
        var signinkey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret!));

        //Create signature using secret key
        var signingCredentials = new SigningCredentials(signinkey, SecurityAlgorithms.HmacSha256);

        //Create Claims to add JWT
        var claims = new Claim[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email ?? ""),
            new Claim(JwtRegisteredClaimNames.FamilyName, user.LastName ?? ""),
            new Claim(JwtRegisteredClaimNames.GivenName, user.FirstName ?? "")

        };

        //Create Token
        var jwt = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.Now.AddMinutes(60),
            signingCredentials: signingCredentials
        );

        var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);
        return encodedJwt;
    }

    public Models.AppUser CreateUser(Models.AppUser appuser)
    {
        var passwordHash = bcrypt.HashPassword(appuser.Password);
        appuser.Password = passwordHash;

        _context?.Add(appuser);
        _context?.SaveChanges();
        return appuser;
    }

    public Models.AppUser GetCurrentUser()
    {
        return _context?.AppUsers.SingleOrDefault()!;
    }

    public string SignIn(string email, string password)
    {
        var user = _context?.AppUsers.SingleOrDefault(x => x.Email == email);
        var verified = false;

        if(user != null){
            verified = bcrypt.Verify(password, user.Password);
        }

        if(user == null || !verified)
        {
            return string.Empty;
        }

        return BuildToken(user);
    }

    public Models.AppUser GetUserById(int user){
        return _context?.AppUsers.SingleOrDefault(p => p.UserId == user)!;
    }

    public void UpdateUser(Models.AppUser user)
    {
        var existingUser = _context.AppUsers.SingleOrDefault(u => u.UserId == user.UserId);
        if(existingUser != null)
        {
            existingUser.FirstName = user.FirstName;
            existingUser.LastName = user.LastName;
            existingUser.Email = user.Email;
            existingUser.Password = user.Password;
            existingUser.Address = user.Address;
            existingUser.ProfilePicture = user.ProfilePicture;
            existingUser.PhoneNumber = user.PhoneNumber;
            _context.SaveChanges();
        }
    }

    public async Task<Models.AppUser?> GetUserByUsername(string username)
    {
        return await _context.AppUsers
            .Where(x => x.Email == username)
            .SingleOrDefaultAsync();
    }
}