using DAL.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class Repository<TEntity> : ReadOnlyRepository<TEntity>, IRepository<TEntity>
    where TEntity : class
    {
        public Repository(DbContext _Context) : base(_Context)
        {
        }



        #region افزودن یک موجودیت جدید
        public void Add(TEntity entity)
        {
            Entities.Add(entity);
        }
        public Task AddAsync(TEntity entity)
        {
            return Entities.AddAsync(entity).AsTask();
        }
        #endregion




        #region افزودن موجویت های جدید
        /// <summary>
        /// ایجاد چند موجودیت جدید
        /// </summary>
        /// <param name="entities">لیستی از موجودیت ها</param>
        public void AddRange(IEnumerable<TEntity> entities)
        {
            Entities.AddRange(entities);
        }

        /// <summary>
        /// ایجاد چند موجودیت جدید بصورت آسنکرون
        /// </summary>
        /// <param name="entities">لیستی از موجودیت ها</param>
        public Task AddRangeAsync(IEnumerable<TEntity> entities)
        {
            return Entities.AddRangeAsync(entities);
        }
        #endregion




        #region ویرایش موجودیت
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
        public virtual void Update(TEntity entity, params Expression<Func<TEntity, object>>[] UpdatedProperties)
        {
            if (Context.Entry(entity).State == EntityState.Detached)
                Entities.Attach(entity);
            var dbEntry = Context.Entry(entity);

            if (UpdatedProperties != null && UpdatedProperties.Length > 0)
            {
                foreach (var Property in UpdatedProperties)
                    dbEntry.Property(Property).IsModified = true;
            }
            else
                Context.Entry(entity).State = EntityState.Modified;
        }

        #endregion





        #region حذف موجودیت
        /// <summary>
        /// حذف موجودیت با آیدی
        /// </summary>
        /// <param name="id">آیدی موجودیت</param>
        public void Remove(object id)
        {
            if (id == null) return;
            var entity = Entities.Find(id);
            Remove(entity);
        }


        /// <summary>
        /// حذف موجودیت
        /// <para>
        /// اگر امکان حذف نرم افزاری موجود باشد، حذف نرم افزاری انجام می شود. 
        /// در غیر اینصورت حذف فیزی کی رخ می دهد.
        /// </para>
        /// <para>
        /// حذف نرم افزاری زمانی اتفاق میفتد که در موجودیت مورد نظر 
        /// پروپرتی با نام IsDeleted وجود داشته باشد.
        /// </para>
        /// </summary>
        /// <param name="entity">موجودیت</param>
        public void Remove(TEntity entity)
        {
            if (entity == null) return;

            #region حذف نرم افزاری - soft delete
            //اگر پروپرتی با نام IsDeleted وجود داشت.
            Type myType = typeof(TEntity);
            PropertyInfo IsDeletedPropery = myType.GetProperty("IsDeleted", BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            if (IsDeletedPropery != null && IsDeletedPropery.PropertyType == typeof(bool))
            {
                IsDeletedPropery.SetValue(entity, true);
                Update(entity);
                return;
            }
            #endregion

            #region حذف فیزیکی
            if (Context.Entry(entity).State == EntityState.Detached)
                Entities.Attach(entity);
            Entities.Remove(entity);
            #endregion
        }


        /// <summary>
        /// حذف چند موجودیت
        /// <para>
        /// اگر امکان حذف نرم افزاری موجود باشد، حذف نرم افزاری انجام می شود. 
        /// در غیر اینصورت حذف فیزی کی رخ می دهد.
        /// </para>
        /// <para>
        /// حذف نرم افزاری زمانی اتفاق میفتد که در موجودیت مورد نظر 
        /// پروپرتی با نام IsDeleted وجود داشته باشد.
        /// </para>
        /// </summary>
        /// <param name="entities">لیستی از موجودیت ها</param>
        public void RemoveRange(IEnumerable<TEntity> entities)
        {
            #region حذف نرم افزاری - soft delete
            //اگر پروپرتی با نام IsDeleted وجود داشت.
            Type myType = typeof(TEntity);
            PropertyInfo IsDeletedPropery = myType.GetProperty("IsDeleted", BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            if (IsDeletedPropery != null && IsDeletedPropery.PropertyType == typeof(bool))
            {
                foreach (var item in entities)
                {
                    IsDeletedPropery.SetValue(item, true);
                    Update(item);
                }
                return;
            }
            #endregion

            #region حذف فیزیکی
            Entities.RemoveRange(entities);
            #endregion
        }


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
            return Entities.Where(filterExpression).UpdateFromQuery(updateExpression);
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
            return Entities.Where(filterExpression).UpdateFromQueryAsync(updateExpression);
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
            return Entities.Where(filterExpression).DeleteFromQuery();
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
            return Entities.Where(filterExpression).DeleteFromQueryAsync();
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
            return Entities.DeleteByKey(keyValues);
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
            return Entities.DeleteByKey(entity);
        }
        #endregion


        #endregion


    }



}
