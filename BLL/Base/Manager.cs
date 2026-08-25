using BLL.Interface;
using DAL;
using DAL.Interface;
using DTO.Base;
using Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace BLL
{

    public class Manager<TEntity, TContext> : IManager<TEntity, TContext> 
        where TEntity : class
        where TContext : DbContext
    {
        private DbContexts context;

        public IUnitOfWork<TEntity, TContext> UOW { get; set; }
        public Manager(DbContexts contexts, IHttpContextAccessor httpContextAccessor)
        {
            UOW = new UnitOfWork<TEntity, TContext>(contexts, httpContextAccessor);
        }

        public Manager(DbContexts context)
        {
            this.context = context;
        }



        #region توابع عمومی


        #region گرفتن اطلاعات موجودیت
        public virtual IEnumerable<TEntity> GetAll()
        {
            return UOW.Entities.GetAll();
        }

        public virtual TEntity GetById(object id)
        {
            if (id == null) return null;
            return UOW.Entities.GetById(id);
        }

        public virtual async Task<TEntity> GetByIdAsync(object id)
        {
            if (id == null) return null;
            return await UOW.Entities.GetByIdAsync(id);
        }
        #endregion



        #region ایجاد موجودیت
        public virtual BaseResult Create(TEntity entity)
        {
            UOW.Entities.Add(entity);
            var IsSuccess = UOW.Commit();
            var IdProperty = entity.GetType().GetProperty("Id").GetValue(entity, null);
            return new BaseResult
            {
                Status = IsSuccess,
                Message = IsSuccess ? null : "ذخیره اطلاعات با خطا همراه بوده است!",
                Model = IsSuccess ? IdProperty : null
            };
        }

        public virtual async Task<BaseResult> CreateAsync(TEntity entity)
        {
            await UOW.Entities.AddAsync(entity);
            var IsSuccess = await UOW.CommitAsync();
            var IdProperty = entity.GetType().GetProperty("Id").GetValue(entity, null);
            return new BaseResult
            {
                Status = IsSuccess,
                Message = IsSuccess ? null : "ذخیره اطلاعات با خطا همراه بوده است!",
                Model = IsSuccess ? IdProperty : null
            };
        }

        public virtual BaseResult CreateRange(IEnumerable<TEntity> entities)
        {
            UOW.Entities.AddRange(entities);
            var IsSuccess = UOW.Commit();
            return new BaseResult
            {
                Status = IsSuccess,
                Message = IsSuccess ? null : "ذخیره اطلاعات با خطا همراه بوده است!"
            };
        }

        public virtual async Task<BaseResult> CreateRangeAsync(IEnumerable<TEntity> entities)
        {
            await UOW.Entities.AddRangeAsync(entities);
            var IsSuccess = await UOW.CommitAsync();
            return new BaseResult
            {
                Status = IsSuccess,
                Message = IsSuccess ? null : "ذخیره اطلاعات با خطا همراه بوده است!"
            };
        }

        #endregion



        #region بروزرسانی موجودیت
        /// <summary>
        /// ویرایش  موجودیت
        /// <para>
        /// var book = GetById(1);
        /// book.Title = "My new title";
        /// repository.Update(book);
        /// </para>
        /// </summary>
        /// <param name="entity">موجودیت مورد نظر</param>
        public virtual BaseResult Update(TEntity entity)
        {
            UOW.Entities.Update(entity);
            var IsSuccess = UOW.Commit();
            return new BaseResult
            {
                Status = IsSuccess,
                Message = IsSuccess ? "ذخیره اطلاعات با موفقیت انجام شد." : "ذخیره اطلاعات با خطا همراه بوده است!"
            };
        }


        /// <summary>
        /// ویرایش  کل موجودیت یا چند فیلد خاص از یک موجودیت
        /// <para>
        /// var book = GetById(1);
        /// book.Title = "My new title";
        /// repository.Update(book, b => b.Title);
        /// </para>
        /// </summary>
        /// <param name="entity">موجودیت مورد نظر</param>
        /// <param name="UpdatedProperties">فیلد هایی که اپدیت شدند</param>
        public virtual BaseResult Update(TEntity entity, params Expression<Func<TEntity, object>>[] UpdatedProperties)
        {
            UOW.Entities.Update(entity, UpdatedProperties);
            var IsSuccess = UOW.Commit();
            return new BaseResult
            {
                Status = IsSuccess,
                Message = IsSuccess ? null : "ذخیره اطلاعات با خطا همراه بوده است!"
            };
        }

        #endregion



        #region حذف موجودیت
        public virtual bool Delete(object id)
        {
            if (id == null) return false;
            UOW.Entities.Remove(id);
            return UOW.Commit();
        }

        public virtual bool Delete(TEntity entity)
        {
            UOW.Entities.Remove(entity);
            return UOW.Commit();
        }

        public virtual bool DeleteRange(IEnumerable<TEntity> entities)
        {
            UOW.Entities.RemoveRange(entities);
            return UOW.Commit();
        }

        #endregion

        #endregion



        #region توابع مستقیم برای اعمال روی دیتابیس بدون نیاز به کامیت

        #region آپدیت مستقیم روی دیتابیس بدون نیاز به کامیت
        /// <summary>
        /// آپدیت مستقیم روی دیتابیس بدون نیاز به کامیت
        /// <para>example : </para>
        /// <para>
        /// UpdateWithCommit(x => x.Id == 1, x => new User {IsEnabled = false})
        /// </para>
        /// </summary>
        /// <param name="filterExpression">شرط برای رکوردهایی که باید ویرایش شوند</param>
        /// <param name="updateExpression">فیلدهایی که باید آپدیت شوند</param>
        /// <returns>تعداد آیتم های آپدیت شده</returns>
        public virtual int UpdateWithCommit(Expression<Func<TEntity, bool>> filterExpression, Expression<Func<TEntity, TEntity>> updateExpression)
        {
            return UOW.Entities.UpdateWithCommit(filterExpression, updateExpression);
        }



        /// <summary>
        /// آپدیت مستقیم روی دیتابیس بدون نیاز به کامیت بصورت آسنکرون
        /// <para>example : </para>
        /// <para>
        /// UpdateWithCommit(x => x.Id == 1, x => new User {IsEnabled = false})
        /// </para>
        /// </summary>
        /// <param name="filterExpression">شرط برای رکوردهایی که باید ویرایش شوند</param>
        /// <param name="updateExpression">فیلدهایی که باید آپدیت شوند</param>
        /// <returns>تعداد آیتم های آپدیت شده</returns>
        public virtual Task<int> UpdateWithCommitAsync(Expression<Func<TEntity, bool>> filterExpression, Expression<Func<TEntity, TEntity>> updateExpression)
        {
            return UOW.Entities.UpdateWithCommitAsync(filterExpression, updateExpression);
        }
        #endregion




        #region حذف مستقیم روی دیتابیس بدون نیاز به کامیت
        /// <summary>
        /// حذف مستقیم روی دیتابیس بدون نیاز به کامیت
        /// <para>example : </para>
        /// <para>
        /// DeleteWithCommit(x => x.Id == 1)
        /// </para>
        /// </summary>
        /// <param name="filterExpression">شرط برای رکوردهایی که باید حذف شوند</param>
        /// <returns>تعداد آیتم های حذف شده</returns>
        public virtual int DeleteWithCommit(Expression<Func<TEntity, bool>> filterExpression)
        {
            return UOW.Entities.DeleteWithCommit(filterExpression);
        }



        /// <summary>
        /// حذف مستقیم روی دیتابیس بدون نیاز به کامیت بصورت آسنکرون
        /// <para>example : </para>
        /// <para>
        /// DeleteWithCommitAsync(x => x.Id == 1)
        /// </para>
        /// </summary>
        /// <param name="filterExpression">شرط برای رکوردهایی که باید حذف شوند</param>
        /// <returns>تعداد آیتم های حذف شده</returns>
        public virtual Task<int> DeleteWithCommitAsync(Expression<Func<TEntity, bool>> filterExpression)
        {
            return UOW.Entities.DeleteWithCommitAsync(filterExpression);
        }
        #endregion



        #region حذف مستقیم از روی دیتابیس با داشتن موجودیت یا PK
        /// <summary>
        /// حذف مستقیم بدون بارگذاری موجودیت روی رم
        /// <para>Example :</para>
        /// <para>
        /// DeleteByKey(customerDTO);
        /// DeleteByKey(1);
        /// </para>
        /// </summary>
        /// <param name="keyValues">کلید اصلی یا ویو مدلی از موجودیت مورد نظر</param>
        /// <returns></returns>
        public virtual int DeleteWithCommit(params object[] keyValues)
        {
            return UOW.Entities.DeleteWithCommit(keyValues);
        }



        /// <summary>
        /// حذف مستقیم از دیتابیس
        /// <para>Example :</para>
        /// <para>
        /// DeleteByKey(customer);
        /// </para>
        /// </summary>
        /// <param name="entity">موجودیت مورد نظر</param>
        /// <returns></returns>
        public virtual int DeleteWithCommit(TEntity entity)
        {
            return UOW.Entities.DeleteWithCommit(entity);
        }
        #endregion


        #endregion


    }
}
