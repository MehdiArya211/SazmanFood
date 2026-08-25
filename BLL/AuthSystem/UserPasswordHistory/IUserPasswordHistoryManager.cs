using Domain.Entities;
using Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Interface
{
    public interface IUserPasswordHistoryManager : IManager<UserPasswordHistory, ApplicationContext>
    {
        bool CheckPasswordHistory(long UserId,string Password, bool StatusSave);
    }
}
