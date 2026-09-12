using Microsoft.EntityFrameworkCore;
using PrimePick.Core.Models;
using PrimePick.Core.Specifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrimePick.Repository
{
    public static class SpecificationsEvaluator<TEntity, TKey> where TEntity : BaseEntity<TKey>
    {
                    
        public static IQueryable<TEntity> GetQuery(IQueryable<TEntity> inputQuery, ISpecifications<TEntity, TKey> spec)
        {
            var query = inputQuery;

           
            if (spec.Criteria != null)
            {
                query = query.Where(spec.Criteria);
            }

            
            query = spec.Includes.Aggregate(query, (currentQuery, includeExpression) => currentQuery.Include(includeExpression));

            if (spec.Sorting is not null)
            {
                query = query.OrderBy(spec.Sorting);
            }

            if (spec.IsEnablePagination)
            {
                query = query.Skip(spec.Skip).Take(spec.Take);
            }

           
            return query;
        }
    }
}
