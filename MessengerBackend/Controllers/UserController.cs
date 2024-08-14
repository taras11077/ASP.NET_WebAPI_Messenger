using System.Security.Cryptography;
using System.Text;
using AutoMapper;
using MessengerBackend.Core.Interfaces;
using MessengerBackend.Core.Models;
using MessengerBackend.DTOs;
using MessengerBackend.Storage;
using MessengerBackend.Requests;

using Microsoft.AspNetCore.Mvc;

namespace MessengerBackend.Controllers;

[ApiController]
[Route("api/users")]
public class UserController : Controller
{
    private readonly IUserService _userService;
    private readonly IMapper _mapper;

    public UserController(IUserService userService, IMapper mapper)
    {
        _userService = userService;
        _mapper = mapper;
    }
    
    // реєстрація користувача
    [HttpPost("register")]
    public async Task<IActionResult> Register(CreateUserRequest request)
    {
        // перевірка валідності моделі (CreateUserRequest)
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        
        try
        {
            var userDb = await _userService.Register(request.Nickname, request.Password);
            
            return Created("user", _mapper.Map<UserDTO>(userDb));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });// 409 Conflict - якщо вже існує юзер з таким нікнеймом
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An unexpected error occurred." }); // 500 Internal Server Error - для всіх інших виключень
        }
    }

    
    // логування користувача
    [HttpPost("login")]
    public async Task<IActionResult> Login(CreateUserRequest request)
    {
        // перевірка валідності моделі (CreateUserRequest)
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        
        try
        {
            var userDb = await _userService.Login(request.Nickname, request.Password);
            HttpContext.Session.SetString("user", userDb.Nickname);
            HttpContext.Session.SetInt32("id", userDb.Id);
            
            return Ok(_mapper.Map<UserDTO>(userDb));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });  // 400 Bad Request - якщо не переданий никнейм або пароль
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message }); // 401 Unauthorized - якщо неправільні никнейм або пароль
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An unexpected error occurred." });  
        }
    }
    

    // отримання всіх користувачів
    [HttpGet]
    public  ActionResult<IEnumerable<UserDTO>> GetUsers([FromQuery] int page = 1, [FromQuery] int size = 10)
    {
        var users =  _userService.GetUsers(page, size);
        return Ok(_mapper.Map<IEnumerable<UserDTO>>(users));
    }
    
    // отримання користувача за id
    [HttpGet("{id}")]
    public async Task<ActionResult<UserDTO>> GetUserById(int id)
    {
        var user = await _userService.GetUserById(id);

        if (user == null)
            return NotFound();
        
        var userDto = _mapper.Map<UserDTO>(user);
        return Ok(userDto);
    }
    
    // отримання користувачів за нікнеймом
    [HttpGet("search/{nickname}")]
    public ActionResult<IEnumerable<User>> SearchUsers(string nickname)
    {
        var users = _userService.SearchUsers(nickname);
        if (!users.Any())
        {
            return NotFound();
        }
        return Ok(_mapper.Map<IEnumerable<UserDTO>>(users));
    }
// видалення користувача    
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteUser([FromRoute] int id)
    {
        await _userService.DeleteUser(id);
        return NoContent();
    }
    
// оновлення користувача
    [HttpPut ("{id}")]
    public async Task<ActionResult<UserDTO>> UpdateUser(int id,CreateUserRequest request)
    {
        var userDb = await _userService.GetUserById(id);
        
        userDb.Nickname = request.Nickname;
        userDb.Password = HashPassword(request.Password);
        userDb.LastSeenOnline = DateTime.UtcNow;
        
        await _userService.UpdateUser(userDb);
        return Ok(_mapper.Map<UserDTO>(userDb));
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

