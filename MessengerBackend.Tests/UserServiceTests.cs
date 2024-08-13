using System.Security.Cryptography;
using System.Text;
using MessengerBackend.Core.Interfaces;
using MessengerBackend.Core.Models;
using MessengerBackend.Core.Services;
using MessengerBackend.Storage;
using Microsoft.EntityFrameworkCore;

namespace MessengerBackend.Tests;

public class UserServiceTests
{
    [Fact]
    public async void UserService_Login_CorrectInput()
    {
        var userService = CreateUserService();

        var expectedUser = new User()
        {
            Nickname = "user3",
            Password = HashPassword("pass3"),
        };
        
        var user = await userService.Login("user3", "pass3");
        
        Assert.Equal(expectedUser, user, new UserComparer());

    }
    
    [Theory]
    [InlineData("")]
    [InlineData("     ")]
    [InlineData(null)]
    public async Task UserService_Login_ThrowsExceptionWhenEmptyField(string data)
    {
        // Assign
        var service = CreateUserService();
     
        // Act
        var exceptionNicknameHandler = async () =>
        {
            await service.Login(data, CorrectPassword);
        };
        var exceptionPasswordHandler = async () =>
        {
            await service.Login(CorrectNickname, data);
        };
     
        // Assert
        await Assert.ThrowsAsync<ArgumentNullException>(exceptionNicknameHandler);
        await Assert.ThrowsAsync<ArgumentNullException>(exceptionPasswordHandler);
    }
    
    
    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    [InlineData("1")]
    [InlineData("@")]
    public async Task UserService_Register_ThrowsExceptionWhenIncorrectNickname(string nickname)
    {
        var service = CreateUserService();
        
        var exceptionHandler = async () =>
        {
            await service.Register(nickname, CorrectPassword);
        };

        await Assert.ThrowsAsync<ArgumentException>(exceptionHandler);
    }
    
    private const string CorrectNickname = "nickname";
    private const string CorrectPassword = "0000";
    
    
    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    [InlineData("1")]
    [InlineData("9999")]
    [InlineData("ghsfghsegfsheg")]
    public async Task UserService_Register_ThrowsExceptionWhenIncorrectPassword(string password)
    {
        var service = CreateUserService();
        
        var exceptionHandler = async () =>
        {
            await service.Register(CorrectNickname, password);
        };

        await Assert.ThrowsAsync<ArgumentException>(exceptionHandler);
    }
    
    
    
    
    private IUserService CreateUserService()
    {
        var options = new DbContextOptionsBuilder<MessengerContext>()
            .UseSqlServer("Data Source = (localdb)\\MSSQLLocalDB; Initial Catalog = MessengerDB; Integrated Security = True; Connect Timeout = 30; Encrypt = False; Trust Server Certificate=False; Application Intent = ReadWrite; Multi Subnet Failover=False")
            .Options;
        
        var context = new MessengerContext(options);
        var repository = new Repository(context);

        return new UserService(repository);
    }
    
    
    
    // метод хешування пароля
    private string HashPassword(string password)
    {
        using (var sha256 = SHA256.Create())
        {
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }
    }
}

class UserComparer : IEqualityComparer<User>
{
    public bool Equals(User x, User y)
    {
        if (ReferenceEquals(x, y)) return true;
        if (ReferenceEquals(x, null)) return false;
        if (ReferenceEquals(y, null)) return false;
        if (x.GetType() != y.GetType()) return false;
        return x.Nickname == y.Nickname && x.Password == y.Password;
    }

    public int GetHashCode(User obj)
    {
        return HashCode.Combine(obj.Nickname, obj.Password);
    }
}