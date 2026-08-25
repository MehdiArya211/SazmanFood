using DAL.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{

    public class ReadOnlyRepository<TEntity> : IReadOnlyRepository<TEntity> where TEntity : class
    {
        protected readonly DbContext Context;
        protected readonly DbSet<TEntity> Entities;

        public ReadOnlyRepository(DbContext _Context)
        {
            Context = _Context;
            Entities = Context.Set<TEntity>();
        }



        #region اعمال شرط ها و جوین ها و مرتب سازی و صفحه بندی

        /// <summary>
        /// اعمال شرط ها و مرتب سازی و صفحه بندی
        /// <para>
        /// GetQueryable(x => x.id == 1 , x => x.OrderBy(a => a.id).ThenByDescending(a => a.Amount) , 5, 2)
        /// </para>
        /// </summary>
        /// <param name="filter">شرط</param>
        /// <param name="orderBy">مرتب سازی</param>
        /// <param name="skip">تعداد آیتم هایی که باید عبور شود</param>
        /// <param name="take">تعداد آیتم هایی که باید گرفته شود</param>
        /// <returns>کوئری از آیتم ها</returns>
        protected virtual IQueryable<TEntity> GetQueryable(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            int? skip = null,
            int? take = null,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> includeExpressions = null
            )
        {
            IQueryable<TEntity> query = Entities;

            if (filter != null)
            {
                query = query.Where(filter);
            }


            #region آیتم هایی که حذف نرم افزاری شده اند را در خروجی نشان ندهد
            //اگر پروپرتی با نام IsDeleted وجود داشت.
            //Type myType = typeof(TEntity);
            //PropertyInfo IsDeletedPropery = myType.GetProperty("IsDeleted", BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            //if (IsDeletedPropery != null && IsDeletedPropery.PropertyType == typeof(bool))
            //    query = query.Where(x => (bool)IsDeletedPropery.GetValue(x, null) == false);
            #endregion


            if (orderBy != null)
            {
                query = orderBy(query);
            }

            if (skip.HasValue)
            {
                query = query.Skip(skip.Value);
            }

            if (take.HasValue)
            {
                query = query.Take(take.Value);
            }

            if (includeExpressions != null)
            {
                query = includeExpressions(query);
            }

            return query;
        }



        ///// <summary>
        ///// اعمال شرط ها و جوین ها و مرتب سازی و صفحه بندی
        ///// </summary>
        ///// <param name="filter">شرط</param>
        ///// <param name="orderBy">مرتب سازی</param>
        ///// <param name="includeExpressions">جوین ها</param>
        ///// <param name="skip">تعداد آیتم هایی که باید عبور شود</param>
        ///// <param name="take">تعداد آیتم هایی که باید گرفته شود</param>
        ///// <returns>کوئری از آیتم ها</returns>
        //protected virtual IQueryable<TEntity> GetQueryable(
        //    Expression<Func<TEntity, bool>> filter = null,
        //    Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
        //    int? skip = null,
        //    int? take = null,
        //    //string includeProperties = null, // جوین با استفاده از اسم پروپرتی ها
        //    //params Expression<Func<TEntity, object>>[] includeExpressions
        //    Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null
        //    )
        //{
        //    IQueryable<TEntity> query = GetQueryable(filter, orderBy, skip, take);
        //    if (include != null /*&& includeExpressions.Count() > 0*/)
        //    {
        //        //var include = includeExpressions
        //        //  .Aggregate<Expression<Func<TEntity, object>>, IQueryable<TEntity>>
        //        //   (query, (current, expression) => current.Include(expression));


        //        //foreach (var expr in includeExpressions)
        //        //{
        //        //    query.Include(expr);
        //        //}


        //        query = include(query);

        //    }

        //    //// جوین با استفاده از اسم پروپرتی ها
        //    //foreach (var includeProperty in includeProperties.Split
        //    //    (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
        //    //{
        //    //    query = query.Include(includeProperty);
        //    //}

        //    return query;
        //}





        /// <summary>
        /// اعمال جوین ها
        /// </summary>
        /// <param name="includeExpressions">جوین ها</param>
        /// <returns>کوئری از آیتم ها</returns>
        protected virtual IQueryable<TEntity> GetIncluds(
            IQueryable<TEntity> query,
            //params Expression<Func<TEntity, object>>[] includeExpressions
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> includeExpressions = null
            )
        {
            if (includeExpressions != null/* && includeExpressions.Count() > 0*/)
            {
                //var include = includeExpressions
                //  .Aggregate<Expression<Func<TEntity, object>>, IQueryable<TEntity>>
                //   (query, (current, expression) => current.Include(expression));
                //return include;

                query = includeExpressions(query);
            }
            return query;
        }
        #endregion




        #region GetAll
        /// <summary>
        /// خواندن همه موجودیت ها 
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerable<TEntity> GetAll()
        {
            #region آیتم هایی که حذف نرم افزاری شده اند را در خروجی نشان ندهد
            //اگر پروپرتی با نام IsDeleted وجود داشت.
            //Type myType = typeof(TEntity);
            //PropertyInfo IsDeletedPropery = myType.GetProperty("IsDeleted", BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            //if (IsDeletedPropery != null && IsDeletedPropery.PropertyType == typeof(bool))
            //    return Entities.Where(x => (bool)IsDeletedPropery.GetValue(x, null) == false);
            #endregion

            return Entities.AsEnumerable();
        }
        #endregion




        #region Get Or Search
        /// <summary>
        /// جستجو بین آیتم ها
        /// </summary>
        /// <param name="filter">عبارت جستجو</param>
        /// <param name="orderBy">عبارت مرتب سازی</param>
        /// <param name="skip">تعداد آیتم هایی که باید از آن عبور شود</param>
        /// <param name="take">تعداد آیتم هایی که باید برگردد</param>
        /// <param name="includeExpressions">وابستگی ها و جوین ها</param>
        /// <returns></returns>
        public virtual IEnumerable<TEntity> Get(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            int? skip = null,
            int? take = null,
            //params Expression<Func<TEntity, object>>[] includeExpressions
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null
            )
        {
            return GetQueryable(filter, orderBy, skip, take, include).AsEnumerable();
        }
        #endregion




        #region گرفتن خروجی بصورت مدل دلخواه - GetDTO
        /// <summary>
        /// گرفتن لیستی از مدل دلخواه
        /// </summary>
        /// <typeparam name="TResult">نوع مدل بازگشتی</typeparam>
        /// <param name="selector">انتخاب کننده برای پروپرتی های مورد نظر</param>
        /// <param name="filter">شرط ها</param>
        /// <param name="orderBy">مرتب سازی</param>
        /// <param name="skip">تعداد ایتم هایی که باید از آنها عبور شود</param>
        /// <param name="take">تعداد ایتم هایی که باید در خروجی برگردد</param>
        /// <param name="includeExpressions">وابستگی ها و جوین ها</param>
        /// <returns>لیستی از مدل دلخواه</returns>
        public IEnumerable<TResult> GetDTO<TResult>(
            Expression<Func<TEntity, TResult>> selector,
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            int? skip = null,
            int? take = null,
            //params Expression<Func<TEntity, object>>[] includeExpressions
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null
            )
        {
            return GetQueryable(filter, orderBy, skip, take, include)
                            .Select(selector);
        }


        ///// <summary>
        ///// گرفتن لیستی از مدل دلخواه
        ///// </summary>
        ///// <typeparam name="TResult">نوع مدل بازگشتی</typeparam>
        ///// <param name="selector">انتخاب کننده برای پروپرتی های مورد نظر</param>
        ///// <param name="filter">شرط ها</param>
        ///// <param name="orderBy">مرتب سازی</param>
        ///// <param name="skip">تعداد ایتم هایی که باید از آنها عبور شود</param>
        ///// <param name="take">تعداد ایتم هایی که باید در خروجی برگردد</param>
        ///// <returns>لیستی از مدل دلخواه</returns>
        //public IEnumerable<TResult> GetDTO<TResult>(
        //    Expression<Func<TEntity, TResult>> selector,
        //    Expression<Func<TEntity, bool>> filter = null,
        //    Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
        //    int? skip = null,
        //    int? take = null)
        //{
        //    return GetQueryable(filter, orderBy, skip, take)
        //                    .Select(selector);
        //}



        /// <summary>
        /// گرفتن یک مدل دلخواه
        /// </summary>
        /// <typeparam name="TResult">نوع مدل بازگشتی</typeparam>
        /// <param name="selector">انتخاب کننده برای پروپرتی های مورد نظر</param>
        /// <param name="filter">شرط ها</param>
        /// <param name="includeExpressions">وابستگی ها و جوین ها</param>
        /// <returns>یک مدل دلخواه</returns>
        public TResult GetOneDTO<TResult>(
            Expression<Func<TEntity, TResult>> selector,
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null
            )
        {
            return GetQueryable(filter, orderBy, null, null, include)
                            .Select(selector)
                            .FirstOrDefault();
        }



        /// <summary>
        /// گرفتن یک مدل دلخواه بصورت آسنکرون
        /// </summary>
        /// <typeparam name="TResult">نوع مدل بازگشتی</typeparam>
        /// <param name="selector">انتخاب کننده برای پروپرتی های مورد نظر</param>
        /// <param name="filter">شرط ها</param>
        /// <param name="includeExpressions">وابستگی ها و جوین ها</param>
        /// <returns>یک مدل دلخواه</returns>
        public async Task<TResult> GetOneDTOAsync<TResult>(
            Expression<Func<TEntity, TResult>> selector,
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> includeExpressions = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null
            )
        {
            return await GetQueryable(filter, orderBy, null, null, includeExpressions)
                            .Select(selector)
                            .FirstOrDefaultAsync();
        }
        #endregion




        #region SingleOrDefault
        /// <summary>
        /// گرفتن یک موجودیت به همراه وابستگی ها
        /// </summary>
        /// <param name="filter">شرط ها</param>
        /// <param name="includeExpressions">وابستگی ها و جوین ها</param>
        /// <returns></returns>
        public virtual TEntity SingleOrDefault(
            Expression<Func<TEntity, bool>> filter = null,
            //params Expression<Func<TEntity, object>>[] includeExpressions
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> includeExpressions = null
            )

        {
            var temp = GetQueryable(filter);
            return GetIncluds(temp, includeExpressions).SingleOrDefault();
        }

        /// <summary>
        /// گرفتن یک موجودیت به همراه وابستگی ها بصورت آسنکرون
        /// </summary>
        /// <param name="filter">شرط ها</param>
        /// <param name="includeExpressions">وابستگی ها و جوین ها</param>
        /// <returns></returns>
        public virtual async Task<TEntity> SingleOrDefaultAsync(
            Expression<Func<TEntity, bool>> filter = null,
            //params Expression<Func<TEntity, object>>[] includeExpressions
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> includeExpressions = null
            )
        {
            var temp = GetQueryable(filter);
            return await GetIncluds(temp, includeExpressions).SingleOrDefaultAsync();
        }
        #endregion



        #region FirstOrDefault
        /// <summary>
        /// گرفتن اولین موجودیت به همراه وابستگی ها پس از مرتب سازی
        /// </summary>
        /// <param name="filter">شرط ها</param>
        /// <param name="orderBy">مرتب سازی</param>
        /// <param name="includeExpressions">وابستگی ها و جوین ها</param>
        /// <returns></returns>
        public virtual TEntity FirstOrDefault(
           Expression<Func<TEntity, bool>> filter = null,
           Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> includeExpressions = null
            )
        {
            var temp = GetQueryable(filter, orderBy);
            return GetIncluds(temp, includeExpressions).FirstOrDefault();
        }

        /// <summary>
        /// گرفتن اولین موجودیت به همراه وابستگی ها پس از مرتب سازی
        /// </summary>
        /// <param name="filter">شرط ها</param>
        /// <param name="orderBy">مرتب سازی</param>
        /// <param name="includeExpressions">وابستگی ها و جوین ها</param>
        public virtual async Task<TEntity> FirstOrDefaultAsync(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> includeExpressions = null
            )

        {
            var temp = GetQueryable(filter, orderBy);
            return await GetIncluds(temp, includeExpressions).FirstOrDefaultAsync();
        }
        #endregion



        #region GetById
        public virtual TEntity GetById(object id)
        {
            var entity = Entities.Find(id);
            if(entity == null)
                return null;

            #region آیتمی که حذف نرم افزاری شده است را در خروجی نشان ندهد
            //اگر پروپرتی با نام IsDeleted وجود داشت.
            Type myType = typeof(TEntity);
            PropertyInfo IsDeletedPropery = myType.GetProperty("IsDeleted", BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            if (IsDeletedPropery != null && IsDeletedPropery.PropertyType == typeof(bool))
                if ((bool)IsDeletedPropery.GetValue(entity, null) == true)
                    return null;
            #endregion

            return entity;
        }

        public virtual Task<TEntity> GetByIdAsync(object id)
        {
            var entity = Entities.FindAsync(id);

            #region آیتمی که حذف نرم افزاری شده است را در خروجی نشان ندهد
            //اگر پروپرتی با نام IsDeleted وجود داشت.
            Type myType = typeof(TEntity);
            PropertyInfo IsDeletedPropery = myType.GetProperty("IsDeleted", BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            if (IsDeletedPropery != null && IsDeletedPropery.PropertyType == typeof(bool))
                if ((bool)IsDeletedPropery.GetValue(entity.Result, null) == true)
                    return null;
            #endregion

            return entity.AsTask();
        }
        #endregion



        #region Count
        public virtual int Count(Expression<Func<TEntity, bool>> filter = null)
        {
            return GetQueryable(filter).Count();
        }

        public virtual Task<int> CountAsync(Expression<Func<TEntity, bool>> filter = null)
        {
            return GetQueryable(filter).CountAsync();
        }
        #endregion







        #region Max
        public virtual TResult Max<TResult>(Expression<Func<TEntity, bool>> filter = null,
                               Expression<Func<TEntity, TResult>> selector = null)
        {
            return GetQueryable(filter).Max(selector);
        }

        public virtual Task<TResult> MaxAsync<TResult>(Expression<Func<TEntity, bool>> filter = null,
                               Expression<Func<TEntity, TResult>> selector = null)
        {
            return GetQueryable(filter).MaxAsync(selector);
        }
        #endregion



        #region Any
        public virtual bool Any(Expression<Func<TEntity, bool>> filter = null)
        {
            return GetQueryable(filter).Any();
        }

        public virtual Task<bool> AnyAsync(Expression<Func<TEntity, bool>> filter = null)
        {
            return GetQueryable(filter).AnyAsync();
        }
        #endregion

    }
}
