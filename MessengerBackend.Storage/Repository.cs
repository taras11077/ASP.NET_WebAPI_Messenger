using MessengerBackend.Core.Interfaces;
using System.Linq.Expressions;

namespace MessengerBackend.Storage;

public class Repository : IRepository
{
    private readonly MessengerContext _context;

    public Repository(MessengerContext context)
    {
        _context = context;
    }

    public async Task<T> Add<T>(T entity) where T : class
    {
        var obj = _context.Add(entity);
        await _context.SaveChangesAsync();

        return obj.Entity;
    }

    public async Task<T> Update<T>(T entity) where T : class
    {
        _context.Set<T>().Update(entity);
        await _context.SaveChangesAsync();

        return entity;
    }

    public async Task Delete<T>(int id) where T : class
    {
        var entity = await _context.Set<T>().FindAsync(id);
        if (entity != null)
        {
            _context.Set<T>().Remove(entity);
            await _context.SaveChangesAsync();
        }
        else
        {
            throw new ArgumentException("Entity not found");
        }
    }

    public async Task<T> GetById<T>(int id) where T : class
    {
        return await _context.Set<T>().FindAsync(id);
    }

    public IQueryable<T> GetAll<T>() where T : class
    {
        return _context.Set<T>().AsQueryable();
    }

    public async Task<IEnumerable<T>> GetQuery<T>(Expression<Func<T, bool>> func) where T : class
    {
        return _context.Set<T>().Where(func);
    }
}