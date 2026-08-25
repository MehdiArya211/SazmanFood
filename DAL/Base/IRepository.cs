using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Interface
{

    public interface IRepository<TEntity> : IReadOnlyRepository<TEntity> where TEntity : class
    {
        #region افزودن موجودیت جدید
        void Add(TEntity entity);
        Task AddAsync(TEntity entity);
        #endregion


        #region افزودن موجودیت های جدید
        void AddRange(IEnumerable<TEntity> entities);
        Task AddRangeAsync(IEnumerable<TEntity> entities);
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
        void Update(TEntity entity, params Expression<Func<TEntity, object>>[] UpdatedProperties);
        #endregion


        #region حذف موجودیت
        void Remove(object id);
        void Remove(TEntity entity);
        void RemoveRange(IEnumerable<TEntity> entities);
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
        int UpdateWithCommit(Expression<Func<TEntity, bool>> filterExpression, Expression<Func<TEntity, TEntity>> updateExpression);



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
        Task<int> UpdateWithCommitAsync(Expression<Func<TEntity, bool>> filterExpression, Expression<Func<TEntity, TEntity>> updateExpression);
        #endregion




        #region حذف بدون نیاز به کامیت

        /// <summary>
        /// حذف مستقیم روی دیتابیس بدون نیاز به کامیت
        /// <para>example : </para>
        /// <para>
        /// DeleteWithCommit(x => x.Id == 1, x => new User {IsEnabled = false})
        /// </para>
        /// </summary>
        /// <param name="filterExpression">شرط برای رکوردهایی که باید حذف شوند</param>
        /// <returns>تعداد آیتم های حذف شده</returns>
        int DeleteWithCommit(Expression<Func<TEntity, bool>> filterExpression);



        /// <summary>
        /// حذف مستقیم روی دیتابیس بدون نیاز به کامیت بصورت آسنکرون
        /// <para>example : </para>
        /// <para>
        /// DeleteWithCommitAsync(x => x.Id == 1, x => new User {IsEnabled = false})
        /// </para>
        /// </summary>
        /// <param name="filterExpression">شرط برای رکوردهایی که باید حذف شوند</param>
        /// <returns>تعداد آیتم های حذف شده</returns>
        Task<int> DeleteWithCommitAsync(Expression<Func<TEntity, bool>> filterExpression);
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
        int DeleteWithCommit(params object[] keyValues);



        /// <summary>
        /// حذف مستقیم از دیتابیس
        /// <para>Example :</para>
        /// <para>
        /// DeleteByKey(customer);
        /// </para>
        /// </summary>
        /// <param name="entity">موجودیت مورد نظر</param>
        /// <returns></returns>
        int DeleteWithCommit(TEntity entity);
        #endregion

        #endregion





    }


}
