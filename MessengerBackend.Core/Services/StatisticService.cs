using MessengerBackend.Core.Interfaces;
using MessengerBackend.Core.Models;

namespace MessengerBackend.Core.Services;

public class StatisticService : IStatisticService
{
    private readonly IRepository _repository;

    public StatisticService(IRepository repository)
    {
        _repository = repository;
    }
    
    public async Task AddUserStatistic(string nickname)
    {
        var userStatistic = _repository.GetAll<UserRequestStatistic>().FirstOrDefault(x => x.UserName == nickname);
        if (userStatistic == null)
            await _repository.Add(new UserRequestStatistic() { UserName = nickname, RequestCount = 1 });
        else
        {
            userStatistic.RequestCount++;
            await _repository.Update(userStatistic);
        }
    }
    
    public IEnumerable<UserRequestStatistic> GetUserStatistic()
    {
        return _repository.GetAll<UserRequestStatistic>().ToList();
    }
}