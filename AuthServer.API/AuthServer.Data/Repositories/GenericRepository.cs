using AuthServer.Core.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Data.Repositories
{
    public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class
    {
        private readonly DbContext dbcontext;
        private readonly DbSet<TEntity> dbSet;

        public GenericRepository(AppDbContext context)
        {
           dbcontext = context;
           dbSet = context.Set<TEntity>();
        }


        public async Task AddAsync(TEntity entity)
        {
            await dbSet.AddAsync(entity);
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync()
        {
           return await dbSet.ToListAsync();
        }

        public async Task<TEntity> GetByIdAsync(int id)
        {
            var entity = await dbSet.FindAsync(id);
            if (entity != null)
            { 
                dbcontext.Entry(entity).State = EntityState.Detached;  //  EntityState.Detachedservis clasında anlıyacağız
            }
            return entity;
        }

        public void Remove(TEntity entity)
        {
            dbSet.Remove(entity);
        }

        public TEntity Update(TEntity entity)
        {
            dbcontext.Entry(entity).State = EntityState.Modified;

            return entity;
        }

        public IQueryable<TEntity> Where(Expression<Func<TEntity, bool>> predicate)
        {
            return dbSet.Where(predicate); 
        }
    }
}
