using SharedLibary.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Core.Services
{
    public interface IServiceGeneric<TEntity,TDto> where TEntity : class where TDto : class
    {
        Task<Response<TDto>> GetByIdAsync(int id);

        Task<Response<IEnumerable<TDto>>> GetAllAsync();  // IEnumerablede üstünde bidaha sorgu yapamıyoruz 

        Task<Response<IEnumerable<TDto>>> Where(Expression<Func<TEntity, bool>> predicate); // IQueryable üzerinde ise sorgu yapabiliyoruz

        Task<Response<TDto>>AddAsync(TDto entity);

        Task<Response<NoDataDto>>Remove(int id);

        Task<Response<NoDataDto>>Update(TDto entity, int id);
    }
}
