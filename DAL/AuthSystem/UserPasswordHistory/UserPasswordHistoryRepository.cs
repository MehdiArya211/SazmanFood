using DAL.Interface;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class UserPasswordHistoryRepository : Repository<UserPasswordHistory>, IUserPasswordHistoryRepository
    {
        public UserPasswordHistoryRepository(DbContext _Context) : base(_Context)
        {
        }
    }
}
