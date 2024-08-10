using AutoMapper;
using MessengerBackend.Core.Interfaces;
using MessengerBackend.Core.Models;
using MessengerBackend.DTOs;
using MessengerBackend.Storage;
using MessengerBackend.Requests;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LoginRequest = Microsoft.AspNetCore.Identity.Data.LoginRequest;

namespace MessengerBackend.Controllers;

[ApiController]
[Route("api/users")]
public class UserController : Controller
{
    private readonly MessengerContext _context;
    private readonly IMapper _mapper;
    private readonly IUserService _userService;

    public UserController(MessengerContext context, IMapper mapper, IUserService userService)
    {
        _context = context;
        _mapper = mapper;
        _userService = userService;
    }
    
    // реєстрація користувача
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] CreateUserRequest request)
    {
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
            return Conflict(new { message = ex.Message });// 409 Conflict, якщо вже існує юзер з таким нікнеймом
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An unexpected error occurred." }); // 500 Internal Server Error для всіх інших виключень
        }
    }

    
    // логування користувача
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] CreateUserRequest request)
    {
        try
        {
            var userDb = await _userService.Login(request.Nickname, request.Password);
            HttpContext.Session.SetString("user", userDb.Nickname);
            HttpContext.Session.SetInt32("id", userDb.Id);
            
            return Ok(_mapper.Map<UserDTO>(userDb));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });  // 400 Bad Request, якщо не переданий никнейм або пароль
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message }); // 401 Unauthorized, якщо невалідні никнейм або пароль
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An unexpected error occurred." });  
        }
    }
    

    // отримання всіх користувачів
    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserDTO>>> GetUsers()
    {
        var users = await _userService.GetUsers(1,10);
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
    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<User>>> SearchUsers([FromQuery] string nickname)
    {
        var users = await _userService.SearchUsers(nickname);
        if (!users.Any())
        {
            return NotFound();
        }
        return Ok(_mapper.Map<IEnumerable<UserDTO>>(users));
    }
    
    
    
    
    

    
    
    
    
}

