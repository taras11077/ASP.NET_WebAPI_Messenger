using MessengerBackend.Core.Models;

namespace MessengerBackend.Core.Interfaces;

public interface IUserStatisticService
{
    void AddStatistic(string userName);
    IEnumerable<UserRequestStatistic> GetStatistic();
}