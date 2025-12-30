using System;
using System.Linq.Expressions;
using ECommerce.Data.Abstract;
using LinqKit;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Data.Concrete;

public class Repository<T> : IRepository<T> where T : class
{
    protected readonly ECommerceDbContext _context;
    private readonly DbSet<T> _dbSet;

    public Repository(ECommerceDbContext context)
    {
        _context = context;
        _dbSet = _context.Set<T>();
    }


    public async Task<IEnumerable<T>> GetAllAsync()
    {
        var result = await _dbSet.ToListAsync();
        return result;
    }

    public async Task<IEnumerable<T>> GetAllAsync(
        Expression<Func<T, bool>> predicate = null!,
        Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null!,
        bool showIsDeleted = false,
        bool asExpanded = false,
        params Func<IQueryable<T>, IQueryable<T>>[] includes)
    {
        IQueryable<T> query = _dbSet;
        if (showIsDeleted)
        {
            query = query.IgnoreQueryFilters();
        }

        if (asExpanded)
        {
            query = query.AsExpandable();
        }

        if (predicate != null)
        {
            query = query.Where(predicate);
        }

        if (orderBy != null)
        {
            query = orderBy(query);
        }

        if (includes != null && includes.Length > 0)
        {
            query = includes.Aggregate(query, (current, include) => include(current));
        }
        var result = await query.ToListAsync();
        return result;
    }

    public async Task<T> GetAsync(int id)
    {
        var result = await _dbSet.FindAsync(id);
        return result!;
    }

    public async Task<T> GetAsync(
        Expression<Func<T, bool>> predicate,
        bool showIsDeleted = false,
        bool asExpanded = false,
        params Func<IQueryable<T>, IQueryable<T>>[] includes)
    {
        IQueryable<T> query = _dbSet;
        if (showIsDeleted)
        {
            query = query.IgnoreQueryFilters();
        }

        if (asExpanded)
        {
            query = query.AsExpandable();
        }

        if (predicate != null)
        {
            query = query.Where(predicate);
        }

        if (includes != null)
        {
            query = includes.Aggregate(query, (current, include) => include(current));
        }

        var result = await query.FirstOrDefaultAsync();
        return result!;
    }


    public async Task AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
    }

    public void Update(T entity)
    {
        _dbSet.Update(entity);
    }

    public void Remove(T entity)
    {
        _dbSet.Remove(entity);
    }

    public async Task<int> CountAsync()
    {
        var result = await _dbSet.CountAsync();
        return result;
    }

    public async Task<int> CountAsync(Expression<Func<T, bool>> predicate)
    {
        var result = await _dbSet.CountAsync(predicate);
        return result;
    }

    public async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate)
    {
        var result = await _dbSet.AnyAsync(predicate);
        return result;
    }

    public void BatchUpdate(IEnumerable<T> entities)
    {
        _dbSet.UpdateRange(entities);
    }

    public async Task<(IEnumerable<T> Data, int TotalCount)> GetPagedAsync(Expression<Func<T, bool>>? predicate = null, Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null, int skip = 0, int take = 10, bool showIsDeleted = false, bool asExpanded = false, params Func<IQueryable<T>, IQueryable<T>>[] includes)
    {
        IQueryable<T> query = _dbSet;
        if (showIsDeleted)
        {
            query = query.IgnoreQueryFilters();
        }

        if (asExpanded)
        {
            query = query.AsExpandable();
        }
        if (predicate is not null)
        {
            query = query.Where(predicate);
        }
        var totalCount = await query.CountAsync();
        if (orderBy is not null)
        {
            query = orderBy(query);
        }

        query = query.Skip(skip).Take(take);

        if (includes is not null && includes.Length > 0)
        {
            query = includes.Aggregate(query, (current, include) => include(current));
        }
        var data = await query.ToListAsync();
        return (data, totalCount);
    }
}
