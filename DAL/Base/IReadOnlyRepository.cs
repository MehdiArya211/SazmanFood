using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Interface
{

    public interface IReadOnlyRepository<TEntity>
    {
        IEnumerable<TEntity> GetAll();



        IEnumerable<TEntity> Get(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            int? skip = null,
            int? take = null,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> includeExpressions = null);



        IEnumerable<TResult> GetDTO<TResult>(
            Expression<Func<TEntity, TResult>> selector,
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            int? skip = null,
            int? take = null,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> includeExpressions = null);

        //IEnumerable<TResult> GetDTO<TResult>(
        //    Expression<Func<TEntity, TResult>> selector,
        //    Expression<Func<TEntity, bool>> filter = null,
        //    Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
        //    int? skip = null,
        //    int? take = null);

        TResult GetOneDTO<TResult>(
            Expression<Func<TEntity, TResult>> selector,
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> includeExpressions = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null);

        Task<TResult> GetOneDTOAsync<TResult>(
            Expression<Func<TEntity, TResult>> selector,
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> includeExpressions = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null);




        TEntity SingleOrDefault(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> includeExpressions = null);

        Task<TEntity> SingleOrDefaultAsync(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> includeExpressions = null);



        TEntity FirstOrDefault(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> includeExpressions = null);

        Task<TEntity> FirstOrDefaultAsync(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> includeExpressions = null);



        TEntity GetById(object id);

        Task<TEntity> GetByIdAsync(object id);



        int Count(Expression<Func<TEntity, bool>> filter = null);

        Task<int> CountAsync(Expression<Func<TEntity, bool>> filter = null);



        TResult Max<TResult>(Expression<Func<TEntity, bool>> filter = null,
                               Expression<Func<TEntity, TResult>> selector = null);

        Task<TResult> MaxAsync<TResult>(Expression<Func<TEntity, bool>> filter = null,
                               Expression<Func<TEntity, TResult>> selector = null);



        bool Any(Expression<Func<TEntity, bool>> filter = null);

        Task<bool> AnyAsync(Expression<Func<TEntity, bool>> filter = null);
    }
}
