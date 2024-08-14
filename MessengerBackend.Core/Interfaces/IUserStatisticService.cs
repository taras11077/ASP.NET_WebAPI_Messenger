using MessengerBackend.Core.Models;

namespace MessengerBackend.Core.Interfaces;

public interface IUserStatisticService
{
    Task AddStatistic(string userName);
    IEnumerable<UserRequestStatistic> GetStatistic();
}