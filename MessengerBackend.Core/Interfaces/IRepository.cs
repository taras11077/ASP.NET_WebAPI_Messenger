namespace MessengerBackend.Core.Interfaces;
using System.Linq.Expressions;

public interface IRepository
{
    Task<T> Add<T>(T entity) where T : class;
    Task<T> Update<T>(T entity) where T : class;
    Task Delete<T>(int id) where T : class;

    Task<T> GetById<T>(int id) where T : class;
    IQueryable<T> GetAll<T>() where T : class;
    Task<IEnumerable<T>> GetQuery<T>(Expression<Func<T, bool>> func) where T : class;
}