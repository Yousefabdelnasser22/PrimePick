using PrimePick.Core.Models;
using PrimePick.Core.Specifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrimePick.Core.Repository.Contract
{
   public interface IGenericRepository<TEntity,TKey> where TEntity : BaseEntity<TKey> 
    
    { 
        Task<IEnumerable<TEntity>>  GetAllAsync();

        Task<IEnumerable<TEntity>> GetAllAsyncWithSpecs(ISpecifications<TEntity, TKey> specs);

        Task<TEntity> GetByIdAsync(TKey id);

        Task<TEntity> GetByIdAsyncWithSpecs(ISpecifications<TEntity, TKey> specs);

        Task AddAsync(TEntity entity);

        void Update(TEntity entity);

        void Delete(TEntity entity);

    }


    
}
