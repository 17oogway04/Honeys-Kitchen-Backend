using Models = Honeys_Kitchen_backend.Models;

namespace Honeys_Kitchen_backend.Repositories;

public interface IUserRepository
{
    Models.AppUser CreateUser(Models.AppUser user);
    string SignIn(string username, string password);
    Models.AppUser GetCurrentUser();

    Models.AppUser GetUserById(int user);
    void UpdateUser(Models.AppUser user);
    Task<Models.AppUser?> GetUserByUsername(string username);


}