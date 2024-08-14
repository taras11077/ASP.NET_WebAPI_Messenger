using MessengerBackend.Core.Interfaces;
using MessengerBackend.Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace MessengerBackend.Controllers;

[ApiController]
[Route("api/statistic")]
public class UserStatisticController: ControllerBase
{
    private readonly IUserStatisticService _userStatisticService;

    public UserStatisticController(IUserStatisticService userStatService)
    {
        _userStatisticService = userStatService;
    }

    [HttpGet]
    public IEnumerable<UserRequestStatistic> Get()
    {
        return _userStatisticService.GetStatistic();
    }
}