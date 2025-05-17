using LibraryOnlineRentalSystem.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace LibraryOnlineRentalSystem.Repository.Common;

public class GeneralRepository<TEntity, TEntityId> : IRepository<TEntity, TEntityId>
    where TEntity : Entity<TEntityId>
    where TEntityId : EntityId
{
    private readonly DbSet<TEntity> _objs;

    public GeneralRepository(DbSet<TEntity> objs)
    {
        _objs = objs ?? throw new ArgumentNullException(nameof(objs));
    }

    public async Task<List<TEntity>> GetAllAsync()
    {
        return await _objs.ToListAsync();
    }

    public async Task<TEntity> GetByIdAsync(TEntityId id)
    {
        return await _objs
            .Where(x => id.Equals(x.Id)).FirstOrDefaultAsync();
    }

    public async Task<List<TEntity>> GetByIdsAsync(List<TEntityId> ids)
    {
        return await _objs
            .Where(x => ids.Contains(x.Id)).ToListAsync();
    }

    public async Task<TEntity> AddAsync(TEntity obj)
    {
        var ret = await _objs.AddAsync(obj);
        return ret.Entity;
    }

    public void Remove(TEntity obj)
    {
        _objs.Remove(obj);
    }

    public DbSet<TEntity> context()
    {
        return _objs;
    }
}