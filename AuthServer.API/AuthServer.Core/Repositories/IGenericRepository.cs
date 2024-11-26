using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Core.Repositories
{
    public interface IGenericRepository<TEntity> where TEntity : class
    {
        Task<TEntity> GetByIdAsync(int id);

        Task<IEnumerable<TEntity>> GetAllAsync();  // IEnumerablede üstünde bidaha sorgu yapamıyoruz 

        // where(x => x.id>5)
        IQueryable<TEntity> Where(Expression<Func<TEntity, bool>> predicate); // IQueryable üzerinde ise sorgu yapabiliyoruz

        Task AddAsync(TEntity entity);

        void Remove(TEntity entity);

        TEntity Update (TEntity entity);
    }
}
