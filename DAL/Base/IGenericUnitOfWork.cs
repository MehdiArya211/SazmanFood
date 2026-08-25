using Microsoft.EntityFrameworkCore;
using System;

namespace DAL.Interface
{
    public interface IUnitOfWork<TEntity, TContext> : IUnitOfWork, IDisposable 
        where TEntity : class
        where TContext : DbContext
    {
        IRepository<TEntity> Entities { get; }

    }
}
