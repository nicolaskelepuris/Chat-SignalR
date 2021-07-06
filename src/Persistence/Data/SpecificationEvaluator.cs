using System.Linq;
using Domain.Entities.Base;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Data
{
    public class SpecificationEvaluator<TEntity> where TEntity : BaseEntity
    {
        public static IQueryable<TEntity> GetQuery(IQueryable<TEntity> inputQuery, ISpecification<TEntity> specification)
        {
            var query = inputQuery;

            if (specification.Criteria != null)
            {
                query = query.Where(specification.Criteria);
            }

            if (specification.OrderBy != null)
            {
                query = query.OrderBy(specification.OrderBy);
            }

            if (specification.OrderByDescending != null)
            {
                query = query.OrderByDescending(specification.OrderByDescending);
            }

            if (specification.IsPaginationEnabled)
            {
                query = query.Skip(specification.Skip).Take(specification.Take);
            }

            query = specification.Includes.Aggregate(query, (current, include) => current.Include(include));

            if (specification.IncludesByString.Any())
            {
                query = specification.IncludesByString.Aggregate(query, (current, include) => current.Include(include));
            }

            return query;
        }
    }
}