using BLL.Interface;
using Domain.Entities;
using DTO.DataTable;
using DTO.User;
using Microsoft.EntityFrameworkCore;
using DTO.Base;
using Microsoft.AspNetCore.Http;
using Services.SessionServices;
using Utilities;
using LinqKit;
using Domain.Enums;
using Domain.Constants;
using DTO;
using Utilities.Extentions;
using Infrastructure.Data;
using DocumentFormat.OpenXml.InkML;
using Services.RedisService;

namespace BLL
{
    public class UserManager : Manager<User, ApplicationContext>, IUserManager
    {
        protected readonly IHttpContextAccessor httpContextAccessor;
        protected readonly ISession Session;
        private readonly IRedisManager Redis;

        public UserManager(DbContexts context, IHttpContextAccessor httpContextAccessor, IRedisManager redis)
            : base(context, httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            Redis = redis ?? throw new ArgumentNullException(nameof(redis));

            // اگر HttpContext هنوز null بود، Session null می‌ماند
            Session = httpContextAccessor.HttpContext?.Session;
        }



        /// <summary>
        /// گرفتن لیست کاربران برای نمایش در پنل مدیریت
        /// </summary>
        /// <returns></returns>
        public DataTableResponseDTO<UserDataTableDTO> GetDataTableDTO(DataTableSearchDTO searchData, UserFilterDataTableDTO filters)
        {
            return UOW.Users.GetDataTableDTO(searchData, filters);
        }





        /// <summary>
        /// گرفتن اطلاعات پایه کاربر با آیدی
        /// </summary>
        /// <param name="id">آیدی کاربر</param>
        /// <returns></returns>
        public UserBaseInfoDTO GetUserBaseInfo(long? id)
        {
            if (id == null) return null;
            return UOW.Users.GetUserBaseInfo((int)id);
        }




        /// <summary>
        /// تلفن همراه معتبر است؟
        /// </summary>
        /// <param name="Mobile">تلفن همراه</param>
        /// <param name="Id">آیدی کاربر</param>
        /// <returns></returns>
        public bool MobileIsUnique(string Mobile, long? Id = null)
        {
            if (string.IsNullOrWhiteSpace(Mobile))
                return true;

            return UOW.Users.MobileIsUnique(
                Mobile.Trim().ToEnglishNumber(),
                Id);
        }




        /// <summary>
        /// نام کاربری معتبر است؟
        /// </summary>
        /// <param name="Username">نام کاربری</param>
        /// <param name="Id">آیدی کاربر</param>
        /// <returns></returns>
        public bool UsernameIsUnique(string Username, long? Id = null)
        {
            return UOW.Users.UsernameIsUnique(Username, Id);
        }





        /// <summary>
        /// ایجاد کاربر جدید
        /// </summary>
        /// <param name="model">مدل اطلاعات ادمین</param>
        /// <returns></returns>
        public BaseResult Create(UserCreateDTO model)
        {
            if (model.Password != model.RePassword)
                return new BaseResult { Status = false, Message = "تکرار کلمه عبور صحیح نمی باشد!" };

            var User = Session.GetUser();

            var user = new User()
            {
                Name = model.Name?.Trim().ToLower().ToPersianCharacter(),
                Username = model.Username?.Trim().ToLower().ToEnglishNumber(),
                RoleId = model.RoleId,
                Password = model.Password.GetHash(),
                Mobile = model.Mobile?.Trim().ToLower().ToEnglishNumber(),
                Type = model.Type,
                CreatorId = User?.Id,
                OmdOrgId = model.OmdOrgId,
                ProvinceId = model.ProvinceId,
                CityId = model.CityId,

                PersonId = model.PersonId,
                PersonCode = model.PersonCode,
                NationalCode = model.NationalCode,
                OrganGarrisonId = model.OrganGarrisonId,
            };

            return base.Create(user);
        }






        /// <summary>
        /// گرفتن مدل لازم برای ویرایش کاربر
        /// </summary>
        /// <param name="id">آیدی کاربر</param>
        /// <returns></returns>
        public UserEditDTO GetEditDTO(long? id)
        {
            if (id == null) return null;
            return UOW.Users.GetOneDTO<UserEditDTO>(UserEditDTO.Selector, x => x.Id == id);
        }





        /// <summary>
        /// ویرایش کاربر
        /// </summary>
        /// <param name="model">اطلاعات کاربر</param>
        /// <returns></returns>
        public BaseResult Update(UserEditDTO model)
        {
            var User = UOW.Users.FirstOrDefault(x => x.Id == model.Id);

            if (User == null)
                return new BaseResult(false, "کاربر مورد نظر یافت نشد");

            User.Mobile = model.Mobile?.Trim().ToLower().ToEnglishNumber();
            User.Name = model.Name?.Trim().ToLower().ToPersianCharacter();
            User.Username = model.Username?.Trim().ToLower().ToEnglishNumber();
            User.IsEnabled = model.IsEnabled;
            User.Type = model.Type;
            User.RoleId = model.RoleId.Value;
            User.OmdOrgId = model.OmdOrgId;
            User.ProvinceId = model.ProvinceId;
            User.CityId = model.CityId;

            User.PersonId = model.PersonId;
            User.PersonCode = model.PersonCode;
            User.NationalCode = model.NationalCode;
            User.OrganGarrisonId = model.OrganGarrisonId;

            return base.Update(User);
        }





        /// <summary>
        /// ویرایش پروفایل
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public BaseResult UpdateProfile(UserEditProfileDTO model)
        {
            var User = UOW.Users.FirstOrDefault(x => x.Id == model.Id);
            User.Mobile = model.Mobile;
            User.Name = model.Name;
            User.Username = model.Username;
            User.ChangePasswordCycle = model.ChangePasswordCycle;

            return base.Update(User);

        }






        /// <summary>
        /// گرفتن مدل لازم برای ویرایش پروفایل توسط کاربر
        /// </summary>
        /// <param name="id">شناسه کاربر</param>
        /// <returns></returns>
        public UserEditProfileDTO GetEditProfileDTO(long? id)
        {
            if (id == null) return null;
            return UOW.Users.GetOneDTO<UserEditProfileDTO>(UserEditProfileDTO.Selector, x => x.Id == id);
        }





        /// <summary>
        /// تغییر کلمه عبور توسط ادمین
        /// </summary>
        /// <param name="model"> مدل تغییر کلمه عبور</param>
        /// <returns></returns>
        public BaseResult ChangePassword(UserChangePasswordDTO model)
        {
            var User = GetById(model.Id);
            if (User == null)
                return new BaseResult { Status = false, Message = "کاربر یافت نشد" };

            User.Password = model.Password.GetHash();
            User.PasswordIsChanged = false;
            User.RegisteredIpAddress = null;
            User.RegisteredIpDate = null;
            var res = Update(User);
            if (res.Status)
                res.Message = "کلمه عبور با موفقیت تغییر یافت.";
            return res;
        }


        /// <summary>
        ///  گرفتن تعداد کل کاربران
        /// </summary>
        /// <returns></returns>
        public int GetTotalCount()
        {
            return UOW.Users.Count();
        }






        /// <summary>
        ///  حذف کاربر به همراه حذف دسترسی های ثبت شده برای او
        /// </summary>
        /// <param name="id">آیدی کاربر</param>
        /// <returns></returns>
        public override bool Delete(object id)
        {
            try
            {
                UOW.Users.Remove(id);
                return UOW.Commit();
            }
            catch
            {
                return false;
            }
        }




        /// <summary>
        /// تغییر کلمه عبور توسط خود کاربر
        /// </summary>
        /// <param name="model">مدل تغییر کلمه عبور</param>
        /// <returns></returns>
        public BaseResult ProfileChangePassword(UserProfileChangePasswordDTO model)
        {
            var user = GetById(model.Id);

            if (!string.IsNullOrEmpty(model.OldPassword))
            {
                if (user.Password != model.OldPassword.GetHash())
                    return new BaseResult { Status = false, Message = "کلمه عبور فعلی صحیح نمی باشد." };
            }

            if (model.RePassword != model.Password)
                return new BaseResult { Status = false, Message = "کلمه عبور جدید تکرار آن باید یکسان باشند." };

            if (model.OldPassword == model.Password)
                return new BaseResult { Status = false, Message = "کلمه عبور فعلی و جدید نباید یکسان باشند." };

            // مقایسه تعداد کاراکترهای مشترک بین رمز فعلی و جدید
            var passwordAllowedSameCharacters = UOW.Constants.GetPasswordAllowedSameCharacters();
            var commonChars = CommonCharacters(model.OldPassword, model.Password);
            if (commonChars > passwordAllowedSameCharacters)
                return new BaseResult { Status = false, Message = $"کلمه عبور جدید نباید بیشتر از {passwordAllowedSameCharacters} کاراکتر با کلمه عبور فعلی مشترک باشد." };

            user.Password = model.Password.GetHash();
            user.PasswordIsChanged = true;
            user.RegisteredIpAddress = GetClientIpAddress();
            user.RegisteredIpDate = DateTime.Now;
            var res = base.Update(user);
            if (res.Status)
            {
                var sessionUser = Session.GetUser();
                res.Model = sessionUser.PasswordIsChanged == false;
                sessionUser.PasswordIsChanged = true;
                sessionUser.RegisteredIpAddress = user.RegisteredIpAddress;
                sessionUser.RegisteredIpDate = user.RegisteredIpDate;
                Session.RemoveUser();
                Session.SetUser(sessionUser);
            }
            return res;
        }


        private string GetClientIpAddress()
        {
            var ipAddress = httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress;

            if (ipAddress == null)
                return null;

            if (ipAddress.IsIPv4MappedToIPv6)
                ipAddress = ipAddress.MapToIPv4();

            return ipAddress.ToString();
        }


        #region مقایسه تعداد کاراکترهای مشترک بین رمز فعلی و جدید

        private int CommonCharacters(string s1, string s2)
        {
            bool[] matchedFlag = new bool[s2.Length];

            for (int i1 = 0; i1 < s1.Length; i1++)
            {
                for (int i2 = 0; i2 < s2.Length; i2++)
                {
                    if (!matchedFlag[i2] && s1.ToCharArray()[i1] == s2.ToCharArray()[i2])
                    {
                        matchedFlag[i2] = true;
                        break;
                    }
                }
            }

            return matchedFlag.Count(u => u);
        }
        #endregion






        /// <summary>
        /// جستجوی کاربران
        /// </summary>
        /// <param name="word">بخشی از نام یا تلفن</param>
        /// <returns></returns>
        public List<UserSearchDTO> SearchUsers(string word)
        {
            var filter = PredicateBuilder.New<User>();

            filter.Start(x => x.IsEnabled);

            word = word.Trim().ToLower().ToEnglishNumber();
            if (!string.IsNullOrEmpty(word))
                filter.And(x => x.Name.Contains(word) || x.Username.Contains(word) || x.Mobile.Contains(word));
            var model = UOW.Users.GetDTO<UserSearchDTO>(UserSearchDTO.Selector, filter, null, null, 15).ToList();
            return model;
        }




        /// <summary>
        /// جستجوی کاربران یک فروشگاه 
        /// </summary>
        /// <param name="StoreId">شناسه فروشگاه</param>
        /// <returns></returns>
        public List<UserSearchDTO> GetUsersForFactors(UserType? Type)
        {
            var filter = PredicateBuilder.New<User>();
            filter.Start(x => x.IsEnabled);

            if (Type != null)
                filter.And(x => x.Type == Type);

            var model = UOW.Users.GetDTO<UserSearchDTO>(UserSearchDTO.Selector, filter).ToList();
            return model;
        }



        /// <summary>
        /// جستجو کاربر برای سلکتایز
        /// </summary>
        /// <param name="word">متن جستجو</param>
        public List<SelectListDTO> Search(string word)
        {
            #region شرطها
            var filter = PredicateBuilder.New<User>(true);

            if (!string.IsNullOrEmpty(word))
            {
                word = word.Trim().ToLower().ToEnglishNumber();
                filter.And(x => x.Name.Contains(word) || x.Username.Contains(word) || x.Mobile.Contains(word));
            }
            #endregion

            return UOW.Users.GetDTO<SelectListDTO>(SelectListDTO.UserSelector, filter, take: 20).ToList();
        }

        /// <summary>
        /// بررسی وجود کاربر در جدول کاربران بدون بررسی کلمه عبور.
        /// </summary>
        public bool ExistsByUsername(string username)
        {
            username = username?.Trim().ToLower().ToEnglishNumber().ToPersianCharacter();

            return !string.IsNullOrWhiteSpace(username) &&
                   UOW.Users.Any(x => x.Username == username && x.IsDeleted == false);
        }

        /// <summary>
        /// ایجاد کاربر رزروکننده غذا از اطلاعات معتبر سرویس پرسنلی.
        /// نام کاربری و رمز اولیه همان کد پرسنلی است و کاربر در اولین ورود
        /// ملزم به تغییر کلمه عبور خواهد بود.
        /// </summary>
        public BaseResult CreateMealBookerFromPersonnel(PersonalInfDTO person, string initialPassword)
        {
            if (person == null || string.IsNullOrWhiteSpace(person.personalCode))
                return new BaseResult(false, "اطلاعات پرسنلی معتبر نیست.");

            var personCode = person.personalCode
                .Trim()
                .ToEnglishNumber()
                .ToPersianCharacter();

            if (ExistsByUsername(personCode))
                return new BaseResult(false, "حساب کاربری قبلاً ایجاد شده است.");

            if (initialPassword?.Trim().ToEnglishNumber() != personCode)
                return new BaseResult(false, "در اولین ورود، کلمه عبور باید همان کد پرسنلی باشد.");

            var fullName = !string.IsNullOrWhiteSpace(person.FullName)
                ? person.FullName.Trim().ToPersianCharacter()
                : ($"{person.FirstName} {person.LastName}").Trim().ToPersianCharacter();

            int? parsedPersonCode = int.TryParse(personCode, out var personCodeValue)
                ? personCodeValue
                : null;

            int? nationalCode = int.TryParse(
                person.MelliCode?.Trim().ToEnglishNumber(),
                out var nationalCodeValue)
                    ? nationalCodeValue
                    : null;

            var mobile = personCode.Length >= 11
                ? personCode.Substring(personCode.Length - 11)
                : personCode.PadLeft(11, '0');

            var user = new User
            {
                Name = string.IsNullOrWhiteSpace(fullName) ? personCode : fullName,
                Username = personCode,
                Password = personCode.GetHash(),
                Mobile = mobile,
                RoleId = RoleConstant.MealBooker,
                IsEnabled = true,
                PasswordIsChanged = false,
                OmdOrgId = person.UnitCode,
                PersonId = person.Id > 0 ? person.Id : null,
                PersonCode = parsedPersonCode,
                NationalCode = nationalCode,
                CreateDate = DateTime.Now,
                IsDeleted = false
            };

            var result = base.Create(user);
            if (result.Status)
            {
                result.Message =
                    "حساب رزرو غذا ایجاد شد. برای ادامه باید کلمه عبور خود را تغییر دهید.";
            }

            return result;
        }


        public long GetUserIdWithUserName(string UserName)
        {
            return UOW.Users.FirstOrDefault(m => m.Username == UserName && m.IsDeleted == false && m.IsEnabled).Id;
        }
    }
}
