using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace SistemaPresupuestario.DAL.Repositories.Base
{
    /// <summary>
    /// Generic repository interface providing common CRUD operations
    /// </summary>
    /// <typeparam name="T">Entity type</typeparam>
    public interface IRepository<T> where T : class
    {
        /// <summary>
        /// Gets entity by ID
        /// </summary>
        T GetById(Guid id);

        /// <summary>
        /// Gets all entities
        /// </summary>
        IEnumerable<T> GetAll();

        /// <summary>
        /// Gets entities with filtering, ordering, and including related data
        /// </summary>
        IQueryable<T> Get(
            Expression<Func<T, bool>> filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            string includeProperties = "");

        /// <summary>
        /// Gets first entity matching the filter
        /// </summary>
        T GetFirst(Expression<Func<T, bool>> filter, string includeProperties = "");

        /// <summary>
        /// Gets first entity matching the filter or default
        /// </summary>
        T GetFirstOrDefault(Expression<Func<T, bool>> filter, string includeProperties = "");

        /// <summary>
        /// Checks if any entity matches the filter
        /// </summary>
        bool Any(Expression<Func<T, bool>> filter);

        /// <summary>
        /// Counts entities matching the filter
        /// </summary>
        int Count(Expression<Func<T, bool>> filter = null);

        /// <summary>
        /// Adds entity to the repository
        /// </summary>
        void Add(T entity);

        /// <summary>
        /// Adds multiple entities
        /// </summary>
        void AddRange(IEnumerable<T> entities);

        /// <summary>
        /// Updates entity
        /// </summary>
        void Update(T entity);

        /// <summary>
        /// Removes entity
        /// </summary>
        void Remove(T entity);

        /// <summary>
        /// Removes entity by ID
        /// </summary>
        void Remove(Guid id);

        /// <summary>
        /// Removes multiple entities
        /// </summary>
        void RemoveRange(IEnumerable<T> entities);

        /// <summary>
        /// Gets paged results
        /// </summary>
        /// <param name="pageNumber">Page number (1-based)</param>
        /// <param name="pageSize">Number of items per page</param>
        /// <param name="totalCount">Total count of items</param>
        /// <param name="filter">Filter expression</param>
        /// <param name="orderBy">Order by expression</param>
        /// <param name="includeProperties">Properties to include</param>
        /// <returns>Paged results</returns>
        IEnumerable<T> GetPaged(
            int pageNumber,
            int pageSize,
            out int totalCount,
            Expression<Func<T, bool>> filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            string includeProperties = "");
    }
}