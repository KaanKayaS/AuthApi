using AuthServer.Core.Models;
using AuthServer.Core.Repositories;
using AuthServer.Core.Services;
using AuthServer.Core.UnitOfWork;
using AutoMapper.Internal.Mappers;
using Microsoft.EntityFrameworkCore;
using SharedLibary.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Service.Services
{
    public class ServiceGeneric<TEntity, TDto> : IServiceGeneric<TEntity, TDto> where TEntity : class where TDto : class
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IGenericRepository<TEntity> genericRepository;

        public ServiceGeneric(IUnitOfWork unitOfWork, IGenericRepository<TEntity> genericRepository)
        {
            this.unitOfWork = unitOfWork;
            this.genericRepository = genericRepository;
        }

        public async Task<Response<TDto>> AddAsync(TDto entity)
        {
            var newEntity = ObjectMapper.Mapper.Map<TEntity>(entity);

            await genericRepository.AddAsync(newEntity);

            await unitOfWork.SaveAsync();

            var newDto = ObjectMapper.Mapper.Map<TDto>(newEntity);

            return Response<TDto>.Success(newDto, 200);
        }

        public async Task<Response<IEnumerable<TDto>>> GetAllAsync()
        {
           var products = ObjectMapper.Mapper.Map<List<TDto>>(await genericRepository.GetAllAsync());

            return Response<IEnumerable<TDto>>.Success(products, 200);
        }

        public async Task<Response<TDto>> GetByIdAsync(int id)
        {
            var product = await genericRepository.GetByIdAsync(id);

            if (product == null)
            {
                return Response<TDto>.Fail("Id NOT FOUND", 404, true);
            }

            var productDto = ObjectMapper.Mapper.Map<TDto>(product);

            return Response<TDto>.Success(productDto, 200);
           
        }

        public async Task<Response<NoDataDto>> Remove(int id)
        {
           var product = await genericRepository.GetByIdAsync(id);
           if(product == null)
            {
                return Response<NoDataDto>.Fail("Silinecek veri bulunamadı.", 404, true);
            }

              genericRepository.Remove(product);
              await unitOfWork.SaveAsync();

            return Response<NoDataDto>.Success(204); //204 durum kodu nocontent responseın bodysinde data yok
        }

        public async Task<Response<NoDataDto>> Update(TDto entity, int id)
        {
            var product = await genericRepository.GetByIdAsync(id);

            if (product == null)
            {
                return Response<NoDataDto>.Fail("Id Not Found", 404, true);
            }

            var updateEntity = ObjectMapper.Mapper.Map<TEntity>(entity);

            genericRepository.Update(updateEntity);
            await unitOfWork.SaveAsync();

            return Response<NoDataDto>.Success(204); //204 durum kodu nocontent responseın bodysinde data yok

        }

        public async Task<Response<IEnumerable<TDto>>> Where(Expression<Func<TEntity, bool>> predicate)
        {
            // where(x=> x.id>5)
             var list = genericRepository.Where(predicate);

            return Response<IEnumerable<TDto>>
                .Success(ObjectMapper.Mapper.Map<IEnumerable<TDto>>(await list.ToListAsync()), 200);

        }
    }
}
