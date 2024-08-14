using MessengerBackend.Core.Interfaces;
using MessengerBackend.Core.Models;

namespace MessengerBackend.Core.Services;

public class UserStatisticService : IUserStatisticService
{
    private readonly Dictionary<string, int> _userStatistic = new Dictionary<string, int>();
    
    public void AddStatistic(string userName)
    {
        if (_userStatistic.ContainsKey(userName))
            _userStatistic[userName]++;
        else
            _userStatistic[userName] = 1;
        
    }

    public IEnumerable<UserRequestStatistic> GetStatistic()
    {
        return _userStatistic.Select(statistic => new UserRequestStatistic
        {
            UserName = statistic.Key,
            RequestCount = statistic.Value
        });
    }
}