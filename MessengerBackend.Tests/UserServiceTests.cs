
using MessengerBackend.Core.Interfaces;
using MessengerBackend.Core.Models;
using MessengerBackend.Core.Services;
using MessengerBackend.Storage;
using MessengerBackend.Tests.Fakes;
using Microsoft.EntityFrameworkCore;

namespace MessengerBackend.Tests;

// AAA Assign, Act, Assert
public class UserServiceTests
{
    private readonly IUserService _userService;
    private readonly FakeUserRepository _repository;
    
    private const string CorrectNickname = "user3";
    private const string CorrectPassword = "pass3";
    
    public UserServiceTests()
    {
        _repository = new FakeUserRepository();
        _userService = new UserService(_repository);
    }
    
// ======= LOGIN ==================================   
// тест логування користувача   
    [Fact]
    public async Task UserService_Login_CorrectInput()
    {
        // Assign
        var expectedUser = new User()
        {
            Nickname = CorrectNickname,
            Password = (_userService as UserService).HashPassword(CorrectPassword),
        };
        
        // Act
        await _userService.Register(CorrectNickname, CorrectPassword);  // for FakeRepository
        var user = await _userService.Login(CorrectNickname, CorrectPassword);
        
        // Assert
        Assert.Equal(expectedUser, user, new UserComparer());

    }
    
 // тест методу логування на порожні поля   
    [Theory]
    [InlineData("")]
    [InlineData("     ")]
    [InlineData(null)]
    public async Task UserService_Login_ThrowsExceptionWhenEmptyField(string data)
    {
        // Assign

     
        // Act
        var exceptionNicknameHandler = async () =>
        {
            await _userService.Login(data,CorrectPassword);
        };
        var exceptionPasswordHandler = async () =>
        {
            await _userService.Login(CorrectNickname, data);
        };
     
        // Assert
        await Assert.ThrowsAsync<ArgumentNullException>(exceptionNicknameHandler);
        await Assert.ThrowsAsync<ArgumentNullException>(exceptionPasswordHandler);
    }
    
    
// ======= REGISTER ==================================     
// тест валідації нікнейма при реєстрації    
    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("abc")]
    [InlineData(null)]
    public async Task UserService_Register_ThrowsExceptionWhenIncorrectNickname(string nickname)
    {
        // Assign
        
        // Act
        var exceptionHandler = async () =>
        {
            await _userService.Register(nickname, CorrectPassword);
        };

        // Assert
        await Assert.ThrowsAsync<ArgumentException>(exceptionHandler);
    }
    
// тест валідації пароля при реєстрації     
    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    [InlineData("111")]
    public async Task UserService_Register_ThrowsExceptionWhenIncorrectPassword(string password)
    {
        // Assign
        
        // Act
        var exceptionHandler = async () =>
        {
            await _userService.Register(CorrectNickname, password);
        };

        // Assert
        await Assert.ThrowsAsync<ArgumentException>(exceptionHandler);
    }
    
    // ====== SEARCH ================

    [Fact]
    public void SearchUsers_ReturnsUsersWhenNicknameIsCorrect()
    {
        // Arrange
        _repository.Users = new List<User>
        {
            new User { Nickname = "TestUser1" },
        };
        var nickname = "TestUser1";

        // Act
        var users = _userService.SearchUsers(nickname).ToList();

        // Assert
        Assert.Single(users);
        Assert.Equal("TestUser1", users.First().Nickname);
    }

    [Fact]
    public void SearchUsers_ReturnsAllUsersWhenNicknameIsEmpty()
    {
        // Arrange
        _repository.Users = new List<User>
        {
            new User { Nickname = "TestUser1" },
            new User { Nickname = "TestUser2" },
        };
        var nickname = string.Empty;

        // Act
        var usersDb = _userService.SearchUsers(nickname);

        // Assert
        Assert.Equal(2, usersDb.Count());
    }

    [Fact]
    public void SearchUsers_ReturnsEmptyWhenNicknameNotExists()
    {
        // Arrange
        _repository.Users = new List<User>
        {
            new User { Nickname = "TestUser1" },
            new User { Nickname = "TestUser2" },
            new User { Nickname = "TestUser3" }
        };
        var nickname = "NonExistentUser";

        // Act
        var users = _userService.SearchUsers(nickname);

        // Assert
        Assert.Empty(users);
    }

    [Fact]
    public void SearchUsers_ReturnsUsersWhenNicknameIsPartOfOtherNickname()
    {
        // Arrange
        _repository.Users = new List<User>
        {
            new User { Nickname = "TestUser1" },
            new User { Nickname = "TestUser2" },
        };
        
        var nickname = "User1";

        // Act
        var users = _userService.SearchUsers(nickname).ToList();

        // Assert
        Assert.Single(users);
        Assert.Equal("TestUser1", users.First().Nickname);
    }

    [Fact]
    public void SearchUsers_IgnoresCaseWhenSearchingByNickname()
    {
        // Arrange
        _repository.Users = new List<User>
        {
            new User { Nickname = "TestUser1" },
            new User { Nickname = "TestUser2" },
        };
        
        var nickname = "user";

        // Act
        var users = _userService.SearchUsers(nickname);

        // Assert
        Assert.Equal(2, users.Count());
    }
    
// ====== GetUserById ================    
    
    [Fact]
    public async Task GetUserById_ReturnedUser_WhenUserExists()
    {
        // Arrange
        var user = new User { Nickname = "TestUser" };
        await _repository.Add(user);

        // Act
        var userDb = await _userService.GetUserById(1);

        // Assert
        Assert.NotNull(userDb);
        Assert.Equal("TestUser", userDb.Nickname);
    }

    [Fact]
    public async Task GetUserById_ReturnsNull_WhenUserDoesNotExist()
    {
        // Arrange
        
        // Act
        var userDb = await _userService.GetUserById(100);

        // Assert
        Assert.Null(userDb);
    }
    
// ====== UPDATE ================ 

    [Fact]
    public async Task UpdateUser_UpdatedUser_WhenUserExists()
    {
        // Arrange
        var user = new User { Nickname = "OldName" };
        await _repository.Add(user);
        user.Nickname = "NewName";

        // Act
        var userDb = await _userService.UpdateUser(user);

        // Assert
        Assert.Equal("NewName", userDb.Nickname);
    }

    [Fact]
    public async Task UpdateUser_ThrowsException_WhenUserDoesNotExist()
    {
        // Arrange
        var user = new User { Nickname = "NonExistUser" };

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _userService.UpdateUser(user));
    }

// =========   DELETE  =====================

    [Fact]
    public async Task DeleteUser_DeletedUser_WhenUserExists()
    {
        // Arrange
        var user = new User { Nickname = "TestUser" };
        await _repository.Add(user);

        // Act
        await _userService.DeleteUser(1);
        var userDb = await _repository.GetById<User>(1);

        // Assert
        Assert.Null(userDb);
    }

    [Fact]
    public async Task DeleteUser_ThrowsException_WhenUserDoesNotExist()
    {
        // Arrange

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _userService.DeleteUser(1));
    }
// ========================
    
    private IUserService CreateUserService()
    {
        var options = new DbContextOptionsBuilder<MessengerContext>()
            .UseSqlServer("Data Source = (localdb)\\MSSQLLocalDB; Initial Catalog = MessengerDB; Integrated Security = True; Connect Timeout = 30; Encrypt = False; Trust Server Certificate=False; Application Intent = ReadWrite; Multi Subnet Failover=False")
            .Options;
        
        var context = new MessengerContext(options);
        var repository = new Repository(context);
        
        return new UserService(repository);
        //return new UserService(new FakeUserRepository());   // for FakeRepository
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