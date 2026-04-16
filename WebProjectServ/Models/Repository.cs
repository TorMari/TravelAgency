using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace WebProjectServ.Models
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly MyDataContext _context;
        private readonly DbSet<T> _dbSet;

        public Repository(MyDataContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<T?> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(T entity)
        {
            _dbSet.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await GetByIdAsync(id);
            if (entity != null)
            {
                _dbSet.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> Exists(int id)
        {
            var entity = await _dbSet.FindAsync(id);
            return entity != null;
        }

        public async Task<T?> GetByIdWithIncludesAsync(int id, params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _dbSet;
            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            var param = Expression.Parameter(typeof(T), "x");
            var prop = Expression.Property(param, "Id");
            var body = Expression.Equal(prop, Expression.Constant(id));
            var lambda = Expression.Lambda<Func<T, bool>>(body, param);

            return await query.FirstOrDefaultAsync(lambda);
        }
    }
}
