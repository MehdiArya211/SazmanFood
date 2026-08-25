using FajrLog.Domain;
using Microsoft.EntityFrameworkCore;
using Services.RedisService;

namespace BLL.FajrLog
{
    public class FajrLogManager : IFajrLogManager
    {


        private DbContext Context;

        public FajrLogManager(DbContext _Context)
        {
            Context = _Context;
        }



        /// <summary>
        /// افزودن لاگ ها به دیتابیس 
        /// </summary>
        /// <returns></returns>
        public bool CreateRange(List<FajrLogEntity> logs)
        {
            try
            {
                if (logs == null || !logs.Any())
                    return true;
                Context.Set<FajrLogEntity>().AddRange(logs);
                Context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }



    }
}
