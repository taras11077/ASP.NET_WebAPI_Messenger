using System.Linq.Expressions;
using MessengerBackend.Core.Interfaces;
using MessengerBackend.Core.Models;

namespace MessengerBackend.Tests.Fakes;

public class FakeUserRepository : IRepository
{
    public List<User> Users { get; set; }
    private int _id;
    
    public FakeUserRepository()
    {
        Users = new List<User>();
        _id = 0;
    }

    public async Task<T> Add<T>(T entity) where T : class
    {
        if (entity is User user)
        {
            user.Id = ++_id;
            Users.Add(user);
            return await Task.FromResult(entity);
        }
        throw new InvalidOperationException("Entity is not of type User.");
    }

    public async Task<T> Update<T>(T entity) where T : class
    {
        if (entity is User user)
        {
            var existingUser = Users.SingleOrDefault(x => x.Id == user.Id);
            if (existingUser != null)
            {
                existingUser.Nickname = user.Nickname;
                existingUser.Password = user.Password;
                existingUser.CreatedAt = user.CreatedAt;
                existingUser.LastSeenOnline = user.LastSeenOnline;
                return await Task.FromResult(entity);
            }
        }
        throw new InvalidOperationException("User not found.");
    }

    public async Task Delete<T>(int id) where T : class
    {
        var user = Users.SingleOrDefault(x => x.Id == id);
        if (user != null)
        {
            Users.Remove(user);
            await Task.CompletedTask;
        }
        else
        {
            throw new InvalidOperationException("User not found.");
        }
    }

    public async Task<T> GetById<T>(int id) where T : class
    {
        var user = Users.SingleOrDefault(x => x.Id == id);
        return await Task.FromResult(user as T);
    }

    public IQueryable<T> GetAll<T>() where T : class
    {
        return (Users.AsEnumerable() as IEnumerable<T>).AsQueryable<T>();
    }

    public async Task<IEnumerable<T>> GetQuery<T>(Expression<Func<T, bool>> func) where T : class
    {
        if (func is Expression<Func<User, bool>> userFunc)
        {
            var result = Users.AsQueryable().Where(userFunc).Cast<T>();
            return await Task.FromResult(result);
        }
        throw new InvalidOperationException("Invalid expression type.");
    }
}