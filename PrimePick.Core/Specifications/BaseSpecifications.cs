using PrimePick.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace PrimePick.Core.Specifications
{
    public class BaseSpecifications <TEntity, TKey> : ISpecifications<TEntity, TKey> where TEntity : BaseEntity<TKey>
    {
        public Expression<Func<TEntity, bool>> Criteria { get; set; } = null;
        public List<Expression<Func<TEntity, object>>> Includes { get; set; } = new List<Expression<Func<TEntity, object>>>();

        public Expression<Func<TEntity, object>> Sorting { get; set; } = null;
        public int Skip { get; set; }

        public int Take { get; set; }

        public bool IsEnablePagination { get; set; }

      

        public BaseSpecifications(Expression<Func<TEntity, bool>> expression)
        {
            Criteria = expression;
        }
   
        public void ApplyPagination( int skip , int take )
        {
            Skip = skip;
            Take = take;
            IsEnablePagination = true;
        }
        public BaseSpecifications() { }
    }
}
