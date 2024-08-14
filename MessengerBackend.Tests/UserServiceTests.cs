using System.Security.Cryptography;
using System.Text;
using MessengerBackend.Core.Interfaces;
using MessengerBackend.Core.Models;
using MessengerBackend.Core.Services;
using MessengerBackend.Storage;
using Microsoft.EntityFrameworkCore;

namespace MessengerBackend.Tests;

// AAA Assign, Act, Assert
public class UserServiceTests
{
    private const string CorrectNickname = "nickname";
    private const string CorrectPassword = "0000";
    
    
    [Fact]
    public async Task UserService_Login_CorrectInput()
    {
        // Assign
        var userService = CreateUserService();
        var expectedUser = new User()
        {
            Nickname = "user3",
            Password = (userService as UserService).HashPassword("pass3"),
        };
        
        // Act
        var user = await userService.Login("user3", "pass3");
        
        // Assert
        Assert.Equal(expectedUser, user, new UserComparer());

    }
    
    [Theory]
    [InlineData("")]
    [InlineData("     ")]
    [InlineData(null)]
    public async Task UserService_Login_ThrowsExceptionWhenEmptyField(string data)
    {
        // Assign
        var userService = CreateUserService();
     
        // Act
        var exceptionNicknameHandler = async () =>
        {
            await userService.Login(data,CorrectPassword);
        };
        var exceptionPasswordHandler = async () =>
        {
            await userService.Login(CorrectNickname, data);
        };
     
        // Assert
        await Assert.ThrowsAsync<ArgumentNullException>(exceptionNicknameHandler);
        await Assert.ThrowsAsync<ArgumentNullException>(exceptionPasswordHandler);
    }
    
    
    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("abc")]
    [InlineData(null)]
    public async Task UserService_Register_ThrowsExceptionWhenIncorrectNickname(string nickname)
    {
        // Assign
        var userService = CreateUserService();
        
        // Act
        var exceptionHandler = async () =>
        {
            await userService.Register(nickname, CorrectPassword);
        };

        // Assert
        await Assert.ThrowsAsync<ArgumentException>(exceptionHandler);
    }
    
    
    
    
    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    [InlineData("111")]
    public async Task UserService_Register_ThrowsExceptionWhenIncorrectPassword(string password)
    {
        // Assign
        var userService = CreateUserService();
        
        // Act
        var exceptionHandler = async () =>
        {
            await userService.Register(CorrectNickname, password);
        };

        // Assert
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