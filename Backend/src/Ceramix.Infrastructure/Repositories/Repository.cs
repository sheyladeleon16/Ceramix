using System.Linq.Expressions;
using Ceramix.Domain.Entities;
using Ceramix.Domain.Interfaces;
using Ceramix.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Ceramix.Infrastructure.Repositories;

public class Repository<T> : IRepository<T> where T : BaseEntity
{
    protected readonly CeramixDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public Repository(CeramixDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public async Task<T?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        await _dbSet.FindAsync(new object[] { id }, ct);

    public async Task<IEnumerable<T>> GetAllAsync(CancellationToken ct = default) =>
        await _dbSet.AsNoTracking().ToListAsync(ct);

    public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate,
                                                 CancellationToken ct = default) =>
        await _dbSet.AsNoTracking().Where(predicate).ToListAsync(ct);

    public async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate,
                                         CancellationToken ct = default) =>
        await _dbSet.AnyAsync(predicate, ct);

    public async Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null,
                                       CancellationToken ct = default) =>
        predicate is null
            ? await _dbSet.CountAsync(ct)
            : await _dbSet.CountAsync(predicate, ct);

    public async Task AddAsync(T entity, CancellationToken ct = default)
    {
        await _dbSet.AddAsync(entity, ct);
        await _context.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(T entity, CancellationToken ct = default)
    {
        _dbSet.Update(entity);
        await _context.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(T entity, CancellationToken ct = default)
    {
        _dbSet.Remove(entity);
        await _context.SaveChangesAsync(ct);
    }
}
