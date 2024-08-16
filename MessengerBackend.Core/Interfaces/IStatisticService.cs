using MessengerBackend.Core.Models;

namespace MessengerBackend.Core.Interfaces;

public interface IStatisticService
{
    Task AddUserStatistic(string userName);
    IEnumerable<UserRequestStatistic> GetUserStatistic();
}