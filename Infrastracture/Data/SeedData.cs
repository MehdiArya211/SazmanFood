using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;
using Utilities.Extentions;

namespace Infrastracture.Data
{
    /// <summary>
    /// دیتای اولیه برای پر کردن دیتابیس
    /// </summary>
    public static class SeedData
    {
        // تا آیدی شماره چند منو ساخته شده است؟
        private static readonly int LastMenuId = 10;

        public static void InitialSeedData(ref ModelBuilder modelBuilder)
        {
            InitialSeedMenus(ref modelBuilder);
            InitialSeedRoles(ref modelBuilder);
            InitialSeedRoleMenus(ref modelBuilder);
            InitialSeedUsers(ref modelBuilder);
            InitialSeedContants(ref modelBuilder);
        }



        #region منو های ادمین - Menus
        public static void InitialSeedMenus(ref ModelBuilder modelBuilder)
        {
            var Menus = new List<Menu>();

            #region داشبورد
            Menus.Add(new Menu
            {
                Id = 1,
                Title = "داشبورد",
                Area = "admin",
                Controller = "dashboard",
                Action = "index",
                Sort = 1,
                ParentId = null,
                MaterialIcon = "dashboard",
                CreateDate = new DateTime(2022, 7, 25)
            });
            #endregion


            #region امکانات مدیریتی
            Menus.Add(new Menu
            {
                Id = 2,
                Title = "امکانات مدیریتی",
                Sort = 7,
                ParentId = null,
                MaterialIcon = "account_circle",
                HasLink = false,
                CreateDate = new DateTime(2022, 7, 25)
            });

            Menus.Add(new Menu
            {
                Id = 3,
                Title = "کاربران",
                Area = "authsystem",
                Controller = "users",
                Action = "index",
                Sort = 1,
                ParentId = 2,
                MaterialIcon = null,
                CreateDate = new DateTime(2022, 7, 25)
            });


            Menus.Add(new Menu
            {
                Id = 4,
                Title = "منو ها",
                Area = "authsystem",
                Controller = "menus",
                Action = "index",
                Sort = 2,
                ParentId = 2,
                MaterialIcon = null,
                CreateDate = new DateTime(2022, 7, 25)
            });

            Menus.Add(new Menu
            {
                Id = 5,
                Title = "نقش ها",
                Area = "authsystem",
                Controller = "roles",
                Action = "index",
                Sort = 3,
                ParentId = 2,
                MaterialIcon = null,
                CreateDate = new DateTime(2022, 7, 25)
            });


            Menus.Add(new Menu
            {
                Id = 10,
                Title = "تنظیمات",
                Area = "shared",
                Controller = "constants",
                Action = "index",
                Sort = 4,
                ParentId = 2,
                MaterialIcon = null,
                CreateDate = new DateTime(2022, 7, 25)
            });
            #endregion


            #region لاگ ها
            Menus.Add(new Menu
            {
                Id = 6,
                Title = "لاگ ها",
                Sort = 10,
                ParentId = null,
                MaterialIcon = "assignment",
                HasLink = false,
                CreateDate = new DateTime(2022, 7, 25)
            });

            Menus.Add(new Menu
            {
                Id = 7,
                Title = "پیام های ارسالی",
                Area = "logsystem",
                Controller = "smslogs",
                Action = "index",
                Sort = 1,
                ParentId = 6,
                MaterialIcon = null,
                CreateDate = new DateTime(2022, 7, 25)
            });

            Menus.Add(new Menu
            {
                Id = 8,
                Title = "ورود و خروج کاربران",
                Area = "logsystem",
                Controller = "loginlogs",
                Action = "index",
                Sort = 3,
                ParentId = 6,
                MaterialIcon = null,
                CreateDate = new DateTime(2022, 11, 17)
            });

            Menus.Add(new Menu
            {
                Id = 9,
                Title = "عملیات کاربران",
                Area = "logsystem",
                Controller = "actionlogs",
                Action = "index",
                Sort = 4,
                ParentId = 6,
                MaterialIcon = null,
                CreateDate = new DateTime(2022, 11, 17)
            });
            #endregion
            modelBuilder.Entity<Menu>()
                       .HasData(Menus);
        }
        #endregion




        #region نقش ها - Role
        public static void InitialSeedRoles(ref ModelBuilder modelBuilder)
        {
            var Roles = new List<Role>();

            Roles.Add(new Role
            {
                Id = 1,
                Title = "ادمین کل",
                CreateDate = new DateTime(2022, 7, 25)
            });

            modelBuilder.Entity<Role>()
                       .HasData(Roles);
        }
        #endregion



        #region دسترسی نقش ها - RoleMenus
        public static void InitialSeedRoleMenus(ref ModelBuilder modelBuilder)
        {
            var RoleMenus = new List<RoleMenu>();

            // به ازای تک تک منو ها دسترسی اضافه شود
            for (int i = 1; i <= LastMenuId; i++)
            {
                RoleMenus.Add(new RoleMenu
                {
                    RoleId = 1,
                    MenuId = i
                });
            }

            modelBuilder.Entity<RoleMenu>()
                       .HasData(RoleMenus);
        }
        #endregion



        #region کاربران - User
        public static void InitialSeedUsers(ref ModelBuilder modelBuilder)
        {
            var Users = new List<User>();

            Users.Add(new User
            {
                Id = 1,
                Name = "ادمین",
                Username = "admin",
                Password = "123123".GetHash(),
                Type = UserType.SuperAdmin,
                RoleId = 1,
                Mobile = "09123456789",
                CreateDate = new DateTime(2022, 7, 25)
            });

            modelBuilder.Entity<User>()
                       .HasData(Users);
        }
        #endregion



        #region مقادیر ثابت - Contants
        public static void InitialSeedContants(ref ModelBuilder modelBuilder)
        {
            var model = new List<Constant>();
            var constantTypes = EnumExtensions.ToEnumViewModel<ConstantType>();
            foreach (var type in constantTypes)
            {
                var dataType = ((ConstantType)type.Id).GetEnumCustomAttribute();
                var tmp = new Constant()
                {
                    Id = (long)(type.Id ?? 0) + 1,
                    CreateDate = new DateTime(2022, 7, 25),
                    Title = type.Title,
                    Type = (ConstantType)type.Id,
                    Value = dataType.DefultValue
                };
                model.Add(tmp);
            }
            modelBuilder.Entity<Constant>()
                       .HasData(model);
        }
        #endregion
    }
}
