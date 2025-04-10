using GenAiPoc.Contracts.Context;
using GenAiPoc.Core.Entities;
using GenAiPoc.Core.Interfaces.IRepository;
using GenAiPoc.Core.Specification;
using GenAiPoc.Infrastructure.Extension;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using XAct.Domain.Repositories;

namespace GenAiPoc.Infrastructure.Repository
{
    public class Repository<T> : IRepository<T> where T : BaseEntity
    {
        protected readonly DbContextGenAiPOC _context;
        protected readonly DbSet<T> _dbSet;

        public Repository(DbContextGenAiPOC context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public async Task<T?> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<List<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<List<T>> GetByConditionAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.Where(predicate).ToListAsync();
        }

        public async Task<T> AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<List<T>> AddRangeAsync(List<T> entities)
        {
            await _dbSet.AddRangeAsync(entities);
            await _context.SaveChangesAsync();
            return entities;
        }

        public async Task UpdateAsync(T entity)
        {
            _dbSet.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(T entity)
        {
            _dbSet.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<int> CountAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.CountAsync(predicate);
        }

        public async Task<List<T>> GetBySpecificationAsync(Core.Specification.ISpecification<T> spec, bool includeAllChildren = false)
        {
            IQueryable<T> query = _context.Set<T>().AsQueryable();

            if (includeAllChildren)
            {
                query = query.IncludeAll(_context);
            }
            else
            {
                foreach (var include in spec.Includes)
                {
                    query = query.Include(include);
                }
            }

            // Apply filtering criteria first for better performance
            if (spec.Criteria != null)
            {
                query = query.Where(spec.Criteria);
            }

            // Apply Includes
            //foreach (var include in spec.Includes)
            //{
            //    query = query.Include(include);
            //}

            //foreach (var thenInclude in spec.ThenIncludes)
            //{
            //    query = query.Include(thenInclude);  
            //}

            // Apply Ordering
            if (spec.OrderBy != null)
            {
                query = spec.OrderBy(query);
            }

            // Apply Pagination
            if (spec.PageNumber.HasValue && spec.PageSize.HasValue)
            {
                query = query.Skip((spec.PageNumber.Value - 1) * spec.PageSize.Value)
                             .Take(spec.PageSize.Value);
            }

            return await query.ToListAsync();
        }

    }

}
