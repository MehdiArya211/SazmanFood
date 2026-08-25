using DAL.Interface;
using Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace DAL
{
    /// <summary>
    /// کلاس جنریک برای منیجر های اصلی موجودیت ها و منیجر پایه
    /// </summary>
    /// <typeparam name="TEntity">موجودیت</typeparam>
    public class UnitOfWork<TEntity, TContext> : UnitOfWork, IUnitOfWork<TEntity, TContext> 
        where TEntity : class
        where TContext : DbContext
    {
        public UnitOfWork(DbContexts contexts,IHttpContextAccessor httpContextAccessor) : base(contexts, httpContextAccessor)
        {
        }
        private Repository<TEntity> _entities;

        public IRepository<TEntity> Entities
        {
            get
            {
                if (_entities == null)
                {
                    if(typeof(TContext) == typeof(LogContext))
                        _entities = new Repository<TEntity>(logContext);
                    else
                        _entities = new Repository<TEntity>(applicationContext);
                }
                return _entities;
            }
        }

    }


}
