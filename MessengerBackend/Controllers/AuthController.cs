using AutoMapper;
using MessengerBackend;
using MessengerBackend.Requests;
using MessengerBackend.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MessengerBackend.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : Controller
{
    private readonly IUserService _userService;
    private readonly IConfiguration _configuration;

    public AuthController(IUserService userService, IConfiguration configuration)
    {
        _userService = userService;
        _configuration = configuration;
    }
    
// реєстрація користувача
    [HttpPost("register")]
    public async Task<IActionResult> Register(CreateUserRequest request)
    {
        return await HandleRequestAsync(request, async () =>
        {
            var userDb = await _userService.Register(request.Nickname, request.Password);
            var jwt = JwtGenerator.GenerateJwt(userDb, _configuration.GetValue<string>("TokenKey")!, DateTime.UtcNow.AddMinutes(5));
        
            HttpContext.Session.SetInt32("id", userDb.Id);

            return Created("token", jwt);
        });
    }

// логування користувача
    [HttpPost("login")]
    public async Task<IActionResult> Login(CreateUserRequest request)
    {
        return await HandleRequestAsync(request, async () =>
        {
            var user = await _userService.Login(request.Nickname, request.Password);
            var jwt = JwtGenerator.GenerateJwt(user, _configuration.GetValue<string>("TokenKey")!, DateTime.UtcNow.AddMinutes(5));

            return Created("token", jwt);
        });
    }
    
    
    // метод для валідації моделі та обробки помилок
    private async Task<IActionResult> HandleRequestAsync(CreateUserRequest request, Func<Task<IActionResult>> action)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
            
        try
        {
            return await action();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message }); // 400 Bad Request - якщо не переданий никнейм або пароль
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message }); // 409 Conflict - якщо вже існує юзер з таким нікнеймом
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message }); // 401 Unauthorized - якщо неправільні никнейм або пароль
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An unexpected error occurred." }); // 500 Internal Server Error - для всіх інших виключень
        }
    }
    
}





// [HttpPost("register")]
// public async Task<ActionResult<string>> Register(CreateUserRequest request)
// {
//     var userDb = await _userService.Register(request.Nickname, request.Password);
//     var jwt = JwtGenerator.GenerateJwt(userDb, _configuration.GetValue<string>("TokenKey")!, DateTime.UtcNow.AddMinutes(5));
//         
//     HttpContext.Session.SetInt32("id", userDb.Id);
//
//     return Created("token", jwt);
// }


// [HttpPost("login")]
// public async Task<ActionResult<string>> Login(CreateUserRequest request)
// {
//     var user = await _userService.Login(request.Nickname, request.Password);
//     var jwt = JwtGenerator.GenerateJwt(user, _configuration.GetValue<string>("TokenKey")!, DateTime.UtcNow.AddMinutes(5));
//
//     return Created("token", jwt);
// }


// // метод для валідації моделі та обробки помилок
// private async Task<IActionResult> HandleRequestAsync(CreateUserRequest request, Func<Task<IActionResult>> action)
// {
//     if (!ModelState.IsValid)
//         return BadRequest(ModelState);
//         
//     try
//     {
//         return await action();
//     }
//     catch (ArgumentException ex)
//     {
//         return BadRequest(new { message = ex.Message });
//     }
//     catch (InvalidOperationException ex)
//     {
//         return Conflict(new { message = ex.Message });
//     }
//     catch (UnauthorizedAccessException ex)
//     {
//         return Unauthorized(new { message = ex.Message });
//     }
//     catch (Exception ex)
//     {
//         return StatusCode(500, new { message = "An unexpected error occurred." });
//     }
// }
//
// // реєстрація користувача
// [HttpPost("register")]
// public Task<IActionResult> Register(CreateUserRequest request)
// {
//     return HandleRequestAsync(request, async () =>
//     {
//         var userDb = await _userService.Register(request.Nickname, request.Password);
//         return Created("user", _mapper.Map<UserDTO>(userDb));
//     });
// }
//
// // логування користувача
// [HttpPost("login")]
// public Task<IActionResult> Login(CreateUserRequest request)
// {
//     return HandleRequestAsync(request, async () =>
//     {
//         var userDb = await _userService.Login(request.Nickname, request.Password);
//         HttpContext.Session.SetString("user", userDb.Nickname);
//         HttpContext.Session.SetInt32("id", userDb.Id);
//         return Ok(_mapper.Map<UserDTO>(userDb));
//     });
// }