namespace App.Domain;

public interface IRepository<T> where T : class
{
    Task<T?> GetByIdAsync(Guid id);
    Task<IEnumerable<T>> GetAllAsync();
    Task AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(Guid id);
    Task<bool> ExistsAsync(Guid id);
    // Soft delete methods
    Task SoftDeleteAsync(Guid id);
    Task RestoreAsync(Guid id);
    Task<IEnumerable<T>> GetActiveAsync(); // Not soft deleted
}