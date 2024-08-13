using System.Linq.Expressions;
using MessengerBackend.Core.Interfaces;
using MessengerBackend.Core.Models;

namespace MessengerBackend.Tests.Fackes;

public class FakeUserRepository : IRepository
{
    private List<User> _users;
    private int id;
    
    public FakeUserRepository()
    {
        _users = new List<User>();
        id = 0;
    }

    public async Task<T> Add<T>(T entity) where T : class
    {
        (entity as User).Id = ++id;
        _users.Add((entity as User));
        return entity;
    }

    public Task<T> Update<T>(T entity) where T : class
    {
        throw new NotImplementedException();
    }

    public Task Delete<T>(int id) where T : class
    {
        throw new NotImplementedException();
    }

    public async Task<T> GetById<T>(int id) where T : class
    {
        return (_users.Single(x => x.Id == id) as T);
    }

    public IQueryable<T> GetAll<T>() where T : class
    {
        return (_users.AsEnumerable() as IEnumerable<T>).AsQueryable<T>();
    }

    public Task<IEnumerable<T>> GetQuery<T>(Expression<Func<T, bool>> func) where T : class
    {
        throw new NotImplementedException();
    }
}