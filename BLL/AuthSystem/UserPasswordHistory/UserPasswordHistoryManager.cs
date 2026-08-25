using BLL.Interface;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities.Extentions;

namespace BLL
{
    public class UserPasswordHistoryManager : Manager<UserPasswordHistory, ApplicationContext>, IUserPasswordHistoryManager
    {
        public UserPasswordHistoryManager(DbContexts contexts, IHttpContextAccessor httpContextAccessor) : base(contexts, httpContextAccessor)
        {
        }

        public bool CheckPasswordHistory(long UserId, string Password, bool StatusSave)
        { 








            var PasswordHash = Password.EncodePasswordMd5();
            int? CountRepeadPass= UOW.Constants.GetCountPasswordHistoryRepaed();
            var PasswordHistorys = UOW.PasswordHistorys.GetAll().Where(_ => _.UserId == UserId).ToList();
           
            if (PasswordHistorys.Count()> CountRepeadPass)
            {
                UOW.PasswordHistorys.Remove(PasswordHistorys.FirstOrDefault());
            }

            // چک میشود که پسورد ورودی برابر سه تا پسورد اخر نباشد
            bool CheckRepitLatThreePassword = PasswordHistorys.OrderByDescending(p => p.Id).Take(CountRepeadPass.Value).Any(p => p.UserId == UserId && p.Password == PasswordHash);
            if (CheckRepitLatThreePassword == false)
            {
                var UserPassHis = new UserPasswordHistory()
                {
                    UserId = UserId,
                    StatusSave = StatusSave,
                    ExpPassword = false,
                    Password = PasswordHash,
                    ChangDate = DateTime.Now,
                };
                base.Create(UserPassHis);
            }
            return CheckRepitLatThreePassword;
        }

    }
}
