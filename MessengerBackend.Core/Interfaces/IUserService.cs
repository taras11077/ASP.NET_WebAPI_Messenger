using MessengerBackend.Core.Models;

namespace MessengerBackend.Core.Interfaces;

public interface IUserService
{
    Task<User> Login(string nickname, string password);
    Task<User> Register(string nickname, string password);
    Task<User> GetUserById(int id);
    
    Task<IEnumerable<User>> GetUsers(int page, int size);
    Task<IEnumerable<User>> SearchUsers(string nickname);
}