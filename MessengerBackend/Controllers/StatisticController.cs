using MessengerBackend.Core.Interfaces;
using MessengerBackend.Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace MessengerBackend.Controllers;

[ApiController]
[Route("api/statistic")]
public class StatisticController: ControllerBase
{
    private readonly IStatisticService _statisticService;

    public StatisticController(IStatisticService statService)
    {
        _statisticService = statService;
    }

    [HttpGet]
    public IEnumerable<UserRequestStatistic> Get()
    {
        return _statisticService.GetUserStatistic();
    }
}