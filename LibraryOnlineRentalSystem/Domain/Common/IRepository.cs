namespace LibraryOnlineRentalSystem.Domain.Common;

public interface IRepository<TEntity, TEntityId>
{
    Task<List<TEntity>> GetAllAsync();
    Task<TEntity> GetByIdAsync(TEntityId id);
    Task<TEntity> AddAsync(TEntity obj);

}