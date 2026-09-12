using PrimePick.Core.Repository.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PrimePick.Core.Repository.Contract;
using PrimePick.Core.Models;
using PrimePick.Repository.Data.Context;
using Microsoft.EntityFrameworkCore;
using PrimePick.Core.Specifications;

namespace PrimePick.Repository.Repositories
{
    public class GenericRepository<TEntity, TKey> : IGenericRepository<TEntity, TKey> where TEntity : BaseEntity<TKey>
    {
        public GenericRepository(AppDBContext context)
        {
            Context = context;
        }

        public AppDBContext Context { get; }

        public async Task AddAsync(TEntity entity)
        {
           await Context.Set<TEntity>().AddAsync(entity);
        }

        public void Delete(TEntity entity)
        {
           Context.Set<TEntity>().Remove(entity);
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            if (typeof(TEntity) == typeof(Product))
            {
                return (IEnumerable<TEntity>) await Context.products.Include(p => p.Brand).Include(p => p.Type).ToListAsync();
            }
            return await Context.Set<TEntity>().ToListAsync();
        }

        public async Task<IEnumerable<TEntity>> GetAllAsyncWithSpecs(ISpecifications<TEntity, TKey> specs)
        {
            return await Applyspecfication(specs).ToListAsync();
        }

        public async Task<TEntity> GetByIdAsync(TKey id)
        {
            if (typeof(TEntity) == typeof(Product))
            {
                return  await Context.products.Include(p => p.Brand).Include(p => p.Type).FirstOrDefaultAsync(p=>p.Id == id as int?) as TEntity;
            }
            return  await Context.Set<TEntity>().FindAsync(id) ;
        }

        public async Task<TEntity> GetByIdAsyncWithSpecs(ISpecifications<TEntity, TKey> specs)
        {
            return await Applyspecfication(specs).FirstOrDefaultAsync();
        }

        public void Update(TEntity entity)
        {
            Context.Set<TEntity>().Update(entity);
        }

        public IQueryable<TEntity> Applyspecfication(ISpecifications<TEntity, TKey> specs)
        {
            return SpecificationsEvaluator<TEntity, TKey>.GetQuery(Context.Set<TEntity>(), specs);
        }
    }
}
