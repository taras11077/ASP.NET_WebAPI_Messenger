using System.Security.Cryptography;
using System.Text;
using MessengerBackend.Core.Interfaces;
using MessengerBackend.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace MessengerBackend.Core.Services;

public class UserService : IUserService
{
    private readonly IRepository _repository;

    public UserService(IRepository repository)
    {
        _repository = repository;
    }
    
// реєстрація користувача   
    public async Task<User> Register(string nickname, string password)
    {
        // валідація вводу
        if (nickname == null || string.IsNullOrEmpty(nickname.Trim()) || nickname.Length < 4 || 
            password == null || string.IsNullOrEmpty(password.Trim()) || password.Length < 4)
            throw new ArgumentException();
        
        // перевірка існування користувача з таким самим ім'ям
        if (_repository.GetAll<User>().Any(u => u.Nickname == nickname))
            throw new InvalidOperationException("User with the same nickname already exists.");
    
        var hashedPassword = HashPassword(password);
    
        // створення нового користувача
        var newUser = new User
        {
            Nickname = nickname,
            Password = hashedPassword,
            CreatedAt = DateTime.UtcNow,
            LastSeenOnline = DateTime.UtcNow
        };
    
        await _repository.Add(newUser);
    
        return newUser;
    }
    
    // метод хешування пароля
    public string HashPassword(string password)
    {
        using (var sha256 = SHA256.Create())
        {
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }
    }
    
    
// логування користувача      
    public async Task<User> Login(string nickname, string password)
    {
        // валідація вводу
        if (nickname == null || string.IsNullOrEmpty(nickname.Trim()) || 
            password == null || string.IsNullOrEmpty(password.Trim()))
            throw new ArgumentNullException();
        
        // перевірка користувача на наявність в базі
        var user = _repository.GetAll<User>().FirstOrDefault(u => u.Nickname == nickname);
        if (user == null)
            throw new UnauthorizedAccessException("Invalid nickname.");
        
        // перевірка пароля
        if (!VerifyPassword(password, user.Password))
            throw new UnauthorizedAccessException("Invalid password.");
        
        // оновлення часу останнього відвідування
        user.LastSeenOnline = DateTime.UtcNow;
        await _repository.Update(user);
    
        return user;
    }
    
    // метод перевірки пароля
    private bool VerifyPassword(string inputPassword, string storedHashedPassword)
    {
        var hashedInputPassword = HashPassword(inputPassword);
        return hashedInputPassword == storedHashedPassword;
    }
    
// отримання користувача за id
    public async Task<User> GetUserById(int id)
    {
        return await _repository.GetById<User>(id);
    }
    
// оновлення користувача
    public async Task<User> UpdateUser(User user)
    {
        return await _repository.Update(user);
    }
    
// видалення користувача
    public async Task DeleteUser(int id)
    {
        await _repository.Delete<User>(id);
    }

    // отримання користувачів з пагінацією 
    public IEnumerable<User> GetUsers(int page, int size)
    {
        return _repository.GetAll<User>()
            .Skip((page - 1) * size)
            .Take(size)
            .ToList();
    }
// отримання користувачів за ім'ям
    public IEnumerable<User> SearchUsers(string nickname)
    {
        return _repository.GetAll<User>()
            .Where(u => u.Nickname.ToLower().Contains(nickname.ToLower()))
            .ToList();
    }
}