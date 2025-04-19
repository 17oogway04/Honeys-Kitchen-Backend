using Honeys_Kitchen_backend.Models;

namespace Honeys_Kitchen_backend.Repositories;

public interface IUserRepository
{
    User CreateUser(User user);
    string SignIn(string username, string password);
    User GetCurrentUser();

    User GetUserById(int user);
    void UpdateUser(User user);
    Task<User?> GetUserByUsername(string username);


}