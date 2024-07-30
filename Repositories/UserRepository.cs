using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Honeys_Kitchen_backend.Migrations;
using Honeys_Kitchen_backend.Models;
using Microsoft.IdentityModel.Tokens;
using bcrypt = BCrypt.Net.BCrypt;


namespace Honeys_Kitchen_backend.Repositories;

public class UserRepository : IUserRepository
{
    private static HoneysKitchenDbContext? _context;
    private static IConfiguration? _config;

    public UserRepository(HoneysKitchenDbContext context, IConfiguration config)
    {
        _context = context;
        _config = config;
    }

    private string BuildToken(User user)
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

    public User CreateUser(User user)
    {
        var passwordHash = bcrypt.HashPassword(user.Password);
        user.Password = passwordHash;

        _context?.Add(user);
        _context?.SaveChanges();
        return user;
    }

    public User GetCurrentUser()
    {
        return _context?.User.SingleOrDefault()!;
    }

    public string SignIn(string email, string password)
    {
        var user = _context?.User.SingleOrDefault(x => x.Email == email);
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

    public User GetUserById(int user){
        return _context?.User.SingleOrDefault(p => p.UserId == user)!;
    }
}