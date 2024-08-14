using MessengerBackend.Core.Interfaces;
using MessengerBackend.Core.Models;

namespace MessengerBackend.Core.Services;

public class UserStatisticService : IUserStatisticService
{
    private readonly IRepository _repository;

    public UserStatisticService(IRepository repository)
    {
        _repository = repository;
    }
    
    public async Task AddStatistic(string nickname)
    {
        var statistics = _repository.GetAll<UserRequestStatistic>().FirstOrDefault(x => x.UserName == nickname);
        if (statistics == null)
            await _repository.Add(new UserRequestStatistic() { UserName = nickname, RequestCount = 1 });
        else
        {
            statistics.RequestCount++;
            await _repository.Update(statistics);
        }
    }

    public IEnumerable<UserRequestStatistic> GetStatistic()
    {
        return _repository.GetAll<UserRequestStatistic>().ToList();
    }
}