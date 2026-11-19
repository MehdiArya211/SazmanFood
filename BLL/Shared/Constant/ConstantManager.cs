using BLL.Interface;
using Domain.Entities;
using Domain.Enums;
using DTO.Base;
using DTO.DataTable;
using Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Utilities.Extentions;

namespace BLL
{
    public class ConstantManager : Manager<Constant, ApplicationContext>, IConstantManager
    {
        public ConstantManager(DbContexts contexts, IHttpContextAccessor httpContextAccessor) : base(contexts, httpContextAccessor)
        {

        }



        /// <summary>
        /// گرفتن لیست برای نمایش در پنل مدیریت
        /// </summary>
        /// <returns></returns>
        public DataTableResponseDTO<Constant> GetDataTableDTO(DataTableSearchDTO searchData)
        {
            return UOW.Constants.GetDataTableDTO(searchData);
        }




        /// <summary>
        /// ویرایش اطلاعات
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="Value"></param>
        /// <returns></returns>
        public BaseResult Update(long Id, string Value)
        {
            var constant = GetById(Id);
            var dataType = constant.Type.GetEnumCustomAttribute();

            #region ولیدیت
            if(dataType.Type == CustomDataType.Number)
            {
                int _val;
                if (!int.TryParse(Value, out _val))
                    return new BaseResult(false, "مقدار عددی وارد شده صحیح نیست!");

                if(dataType.Min != 0 && _val < dataType.Min)
                    return new BaseResult(false, $"مقدار وارد شده نباید کمتر از {dataType.Min} باشد!");
                if (dataType.Max != 0 && _val > dataType.Max)
                    return new BaseResult(false, $"مقدار وارد شده نباید بیشتر از {dataType.Max} باشد!");
            }
            else if(dataType.Type == CustomDataType.String)
            {
                if (dataType.Min != 0 && Value.Length < dataType.Min)
                    return new BaseResult(false, $"تعداد کاراکتر وارد شده نباید کمتر از {dataType.Min} باشد!");
                if (dataType.Max != 0 && Value.Length > dataType.Max)
                    return new BaseResult(false, $"تعداد کاراکتر وارد شده نباید بیشتر از {dataType.Max} باشد!");
            }
            #endregion


            #region تبدیل نوع بولین به متن true یا false برای ذخیره
            if (dataType.Type == CustomDataType.Bool)
            {
                if (Value == "on") Value = "true";
                else Value = "false";
            }
            #endregion


            constant.Value = Value;
            return Update(constant);
        }



        /// <summary>
        /// گرفتن تعداد دفعات مجاز برای لاگین ناموفق
        /// <para>
        /// مقدار پیشفرض 5 است.
        /// </para>
        /// </summary>
        /// <returns></returns>
        public int GetFailedLoginCount()
        {
            return UOW.Constants.GetFailedLoginCount();
        }





        /// <summary>
        /// کاربر هر چند روز کلمه عبور را تغییر دهد
        /// <para>
        /// مقدار پیشفرض 60 است.
        /// </para>
        /// </summary>
        /// <returns></returns>
        public int? GetChangePasswordCycle()
        {
            return UOW.Constants.GetFailedLoginCount();
        }




        /// <summary>
        /// تعداد کاراکترهایی که هنگام تغییر کلمه عبور میتواند با گلمه عبور قبلی مشترک باشد.
        /// <para>
        /// مقدار پیشفرض 4 است.
        /// </para>
        /// </summary>
        /// <returns></returns>
        public int? GetPasswordAllowedSameCharacters()
        {
            return UOW.Constants.GetPasswordAllowedSameCharacters();
        }





        /// <summary>
        /// خواندن مقدار عددی از مقادیر ثابت
        /// </summary>
        /// <param name="Type">نوع مقادیر ثابت مورد نظر</param>
        /// <returns></returns>
        public bool IsFreeFoodReservationEnabled()
        {
            var setting = UOW.Constants.FirstOrDefault(
                x => x.Type == ConstantType.FreeFoodReservationEnabled && x.IsDeleted == false);

            return setting != null &&
                string.Equals(setting.Value?.Trim(), "true",
                    StringComparison.OrdinalIgnoreCase);
        }

        public BaseResult SetFreeFoodReservationEnabled(bool enabled)
        {
            try
            {
                var setting = UOW.Constants.FirstOrDefault(
                    x => x.Type == ConstantType.FreeFoodReservationEnabled && x.IsDeleted == false);

                if (setting == null)
                {
                    return base.Create(new Constant
                    {
                        Title = "رزرو آزاد غذا بدون کنترل سهمیه فردی",
                        Type = ConstantType.FreeFoodReservationEnabled,
                        Value = enabled ? "true" : "false"
                    });
                }

                setting.Value = enabled ? "true" : "false";
                return base.Update(setting);
            }
            catch (Exception)
            {
                return new BaseResult(false,
                    "ذخیره تنظیمات رزرو غذا با خطا همراه شد.");
            }
        }

        public FoodTimingPolicyDTO GetFoodTimingPolicy()
        {
            var setting = UOW.Constants.FirstOrDefault(
                x => x.Type == ConstantType.FoodTimingPolicy && x.IsDeleted == false);

            // حفظ رفتار فعلی برنامه هنگام نبود تنظیمات.
            if (string.IsNullOrWhiteSpace(setting?.Value))
                return new FoodTimingPolicyDTO();

            var sections = setting.Value.Split('|');
            if ((sections.Length != 3 && sections.Length != 5 && sections.Length != 6) ||
                !sections[0].StartsWith("p", StringComparison.Ordinal) ||
                !sections[1].StartsWith("r", StringComparison.Ordinal))
            {
                throw new InvalidOperationException(
                    "تنظیمات زمان‌بندی غذا در پایگاه داده معتبر نیست.");
            }

            var printParts = sections[0].Substring(1).Split(',');
            if (printParts.Length != 2 ||
                (printParts[0] != "0" && printParts[0] != "1") ||
                (sections[1] != "r0" && sections[1] != "r1") ||
                !FoodTimingRules.TryReadTime(printParts[1], out _))
            {
                throw new InvalidOperationException(
                    "تنظیمات ساعت چاپ ژتون معتبر نیست.");
            }

            var days = new List<FoodReservationDeadlineDayDTO>();
            foreach (var token in sections[2].Split(';'))
            {
                var fields = token.Split(',');
                if (fields.Length != 3 ||
                    !int.TryParse(fields[0], out var dayOfWeek) ||
                    !int.TryParse(fields[1], out var daysBefore) ||
                    dayOfWeek < 0 || dayOfWeek > 6 ||
                    daysBefore < 0 || daysBefore > 7 ||
                    !FoodTimingRules.TryReadTime(fields[2], out _))
                {
                    throw new InvalidOperationException(
                        "مقادیر مهلت رزرو هفتگی معتبر نیست.");
                }

                days.Add(new FoodReservationDeadlineDayDTO
                {
                    DayOfWeek = dayOfWeek,
                    DaysBefore = daysBefore,
                    CutoffTime = fields[2]
                });
            }

            if (days.Count != 7 || days.Select(x => x.DayOfWeek).Distinct().Count() != 7)
                throw new InvalidOperationException(
                    "تنظیمات مهلت رزرو باید شامل دقیقاً هفت روز متفاوت باشد.");

            var flags = "1111111111";
            if (sections.Length >= 5)
            {
                if ((sections[3] != "b0" && sections[3] != "b1") ||
                    sections[4].Length != 11 || sections[4][0] != 'm' ||
                    sections[4].Substring(1).Any(x => x != '0' && x != '1'))
                    throw new InvalidOperationException("تنظیمات نمایش و رزرو وعده‌ها معتبر نیست.");
                flags = sections[4].Substring(1);
            }

            var maxPrintCount = 3;
            if (sections.Length == 6 &&
                (!sections[5].StartsWith("c", StringComparison.Ordinal) ||
                 !int.TryParse(sections[5].Substring(1), out maxPrintCount) ||
                 maxPrintCount < 1 || maxPrintCount > 20))
                throw new InvalidOperationException("حداکثر تعداد چاپ ژتون معتبر نیست.");

            return new FoodTimingPolicyDTO
            {
                PrintTimeEnabled = printParts[0] == "1",
                PrintStartTime = printParts[1],
                MaxPrintCount = maxPrintCount,
                ReservationDeadlineEnabled = sections[1] == "r1",
                BlockPastDays = sections.Length >= 5 && sections[3] == "b1",
                BreakfastReservable = flags[0] == '1',
                BreakfastVisible = flags[1] == '1',
                LunchReservable = flags[2] == '1',
                LunchVisible = flags[3] == '1',
                DinnerReservable = flags[4] == '1',
                DinnerVisible = flags[5] == '1',
                DessertReservable = flags[6] == '1',
                DessertVisible = flags[7] == '1',
                SideDishReservable = flags[8] == '1',
                SideDishVisible = flags[9] == '1',
                Days = days
            };
        }

        public BaseResult SaveFoodTimingPolicy(FoodTimingPolicyDTO policy)
        {
            if (policy == null || !FoodTimingRules.TryReadTime(policy.PrintStartTime, out _))
                return new BaseResult(false, "ساعت شروع چاپ ژتون باید در قالب HH:mm باشد.");

            if (policy.MaxPrintCount < 1 || policy.MaxPrintCount > 20)
                return new BaseResult(false, "تعداد مجاز چاپ هر ژتون باید بین ۱ تا ۲۰ باشد.");

            if (policy.Days == null || policy.Days.Count != 7 ||
                policy.Days.Select(x => x.DayOfWeek).Distinct().Count() != 7)
                return new BaseResult(false, "برای هر هفت روز هفته، مهلت رزرو مشخص کنید.");

            foreach (var day in policy.Days)
            {
                if (day.DayOfWeek < 0 || day.DayOfWeek > 6 ||
                    day.DaysBefore < 0 || day.DaysBefore > 7 ||
                    !FoodTimingRules.TryReadTime(day.CutoffTime, out _))
                {
                    return new BaseResult(false,
                        "روز، تعداد روز قبل (۰ تا ۷) یا ساعت پایان رزرو معتبر نیست.");
                }
            }

            // یک رکورد و یک Commit، برای جلوگیری از ثبت ناقص چند تنظیم مستقل.
            var serialized =
                "p" + (policy.PrintTimeEnabled ? "1" : "0") + "," + policy.PrintStartTime +
                "|r" + (policy.ReservationDeadlineEnabled ? "1" : "0") +
                "|" + string.Join(";", policy.Days
                    .OrderBy(x => x.DayOfWeek)
                    .Select(x => $"{x.DayOfWeek},{x.DaysBefore},{x.CutoffTime}")) +
                "|b" + (policy.BlockPastDays ? "1" : "0") +
                "|m" + string.Concat(new[] {
                    policy.BreakfastReservable, policy.BreakfastVisible,
                    policy.LunchReservable, policy.LunchVisible,
                    policy.DinnerReservable, policy.DinnerVisible,
                    policy.DessertReservable, policy.DessertVisible,
                    policy.SideDishReservable, policy.SideDishVisible
                }.Select(x => x ? "1" : "0")) +
                "|c" + policy.MaxPrintCount;

            if (serialized.Length > 300)
                return new BaseResult(false, "مقادیر تنظیمات زمان‌بندی بیش از حد مجاز است.");

            var setting = UOW.Constants.FirstOrDefault(
                x => x.Type == ConstantType.FoodTimingPolicy && x.IsDeleted == false);

            if (setting == null)
            {
                return base.Create(new Constant
                {
                    Title = "ساعت چاپ ژتون و مهلت رزرو هفتگی",
                    Type = ConstantType.FoodTimingPolicy,
                    Value = serialized
                });
            }

            setting.Value = serialized;
            return base.Update(setting);
        }

        public int? GetNumberValue(ConstantType Type)
        {
            return UOW.Constants.GetNumberValue(Type);
        }

        public LoginPageMessageDTO GetLoginPageMessage(bool faceLogin)
        {
            var type = faceLogin ? ConstantType.FaceLoginMessage : ConstantType.PasswordLoginMessage;
            var setting = UOW.Constants.FirstOrDefault(x => x.Type == type && x.IsDeleted == false);
            if (string.IsNullOrWhiteSpace(setting?.Value)) return new LoginPageMessageDTO();
            var parts = setting.Value.Split('|', 4);
            if (parts.Length != 4 || parts[0] != "v1" ||
                !int.TryParse(parts[1], out var size) || size < 12 || size > 28 ||
                !System.Text.RegularExpressions.Regex.IsMatch(parts[2], "^#[0-9a-fA-F]{6}$"))
                return new LoginPageMessageDTO();
            var body = parts[3];
            if (body.Length < 2 || (body[0] != '0' && body[0] != '1'))
                return new LoginPageMessageDTO();
            return new LoginPageMessageDTO
            {
                Text = body.Substring(1), FontSize = size,
                Color = parts[2], Bold = body[0] == '1'
            };
        }

        public BaseResult SaveLoginPageMessage(bool faceLogin, LoginPageMessageDTO message)
        {
            if (message == null || message.FontSize < 12 || message.FontSize > 28 ||
                !System.Text.RegularExpressions.Regex.IsMatch(message.Color ?? "", "^#[0-9a-fA-F]{6}$") ||
                (message.Text?.Length ?? 0) > 270 ||
                (message.Text?.Contains('|') ?? false))
                return new BaseResult(false, "متن، رنگ یا اندازه پیام معتبر نیست (حداکثر ۲۷۰ نویسه). ");

            var type = faceLogin ? ConstantType.FaceLoginMessage : ConstantType.PasswordLoginMessage;
            var value = $"v1|{message.FontSize}|{message.Color}|{(message.Bold ? "1" : "0")}{message.Text?.Trim()}";
            if (value.Length > 300) return new BaseResult(false, "متن پیام بیش از حد مجاز است.");
            var setting = UOW.Constants.FirstOrDefault(x => x.Type == type && x.IsDeleted == false);
            if (setting == null)
                return base.Create(new Constant { Title = faceLogin ? "پیام ورود با چهره" : "پیام ورود با رمز", Type = type, Value = value });
            setting.Value = value;
            return base.Update(setting);
        }





    }
}
