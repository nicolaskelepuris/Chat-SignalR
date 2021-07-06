using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Entities.Base;

namespace Domain.Interfaces
{
    public interface IGenericRepository<T> where T : BaseEntity
    {
        Task<T> GetEntityByIdAsync(Guid id);
        Task<IReadOnlyList<T>> ListAllAsync();
        Task<T> GetEntityAsyncWithSpec(ISpecification<T> spec);
        Task<IReadOnlyList<T>> ListAsyncWithSpec(ISpecification<T> spec);
        Task<int> CountAsync(ISpecification<T> spec);
        void Add(T entity);
        void Update(T entity);
        void Delete(T entity);
    }
}