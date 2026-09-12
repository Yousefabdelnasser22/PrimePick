using PrimePick.Core.Models;
using PrimePick.Core.Repository.Contract;
using PrimePick.Repository.Data.Context;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrimePick.Repository.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private Hashtable _repositories;
        public UnitOfWork(AppDBContext context)
        {
            Context = context;
            _repositories = new Hashtable();
        }

        public AppDBContext Context { get; }

        public async Task<int> CompleteAsync()
        {
            return await Context.SaveChangesAsync();
        }

        public IGenericRepository<TEntity, TKey> Repository<TEntity, TKey>() where TEntity : BaseEntity<TKey>
        {
            var type = typeof(TEntity).Name;
            if (!_repositories.ContainsKey(type))
            {
                var repository = new GenericRepository<TEntity, TKey>(Context);
                _repositories.Add(type, repository);

            }

            return _repositories[type] as IGenericRepository<TEntity, TKey>;
        }
    }
}
