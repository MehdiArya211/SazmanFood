using Domain.Entities;
using Domain.Enums;
using DTO.Base;
using Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Utilities.Extentions;

namespace BLL;

/// <summary>
/// ایجاد خودکار حساب ورود برای پرسنلی که در سهمیه غذا ثبت می‌شود.
/// </summary>
public class ReservationUserProvisioningManager
    : Manager<User, ApplicationContext>,
      IReservationUserProvisioningManager
{
    private const string ReservationRoleTitle = "رزرو غذا";

    public ReservationUserProvisioningManager(
        DbContexts contexts,
        IHttpContextAccessor httpContextAccessor)
        : base(contexts, httpContextAccessor)
    {
    }

    public BaseResult EnsureReservationUser(
        long? personId,
        string personCode,
        string fullName,
        string nationalCode,
        long? organGarrisonId,
        long creatorId,
        int omdOrgId = 0)
    {
        personCode = Normalize(personCode);

        if (string.IsNullOrWhiteSpace(personCode))
            return new BaseResult(false, "برای ساخت حساب کاربری، کد پرسنلی الزامی است.");

        var existingUser = UOW.Users.FirstOrDefault(x =>
            x.IsDeleted == false &&
            ((personId.HasValue && x.PersonId == personId.Value) ||
             x.Username == personCode));

        if (existingUser != null)
            return new BaseResult(true, "حساب کاربری پرسنل قبلاً ایجاد شده است.", existingUser.Id);

        var roleResult = EnsureReservationRole();
        if (!roleResult.Status)
            return roleResult;

        int? parsedNationalCode = null;
        if (int.TryParse(Normalize(nationalCode), out var nationalCodeValue))
            parsedNationalCode = nationalCodeValue;

        int? parsedPersonCode = null;
        if (int.TryParse(personCode, out var personCodeValue))
            parsedPersonCode = personCodeValue;

        var mobile = personCode.Length >= 11
            ? personCode.Substring(personCode.Length - 11, 11)
            : personCode.PadLeft(11, '0');

        var user = new User
        {
            Name = string.IsNullOrWhiteSpace(fullName)
                ? personCode
                : fullName.Trim().ToPersianCharacter(),
            Username = personCode,
            Password = personCode.GetHash(),
            Mobile = mobile,
            RoleId = Convert.ToInt64(roleResult.Model),
            Type = UserType.Other,
            CreatorId = creatorId,
            OmdOrgId = omdOrgId,
            PersonId = personId,
            PersonCode = parsedPersonCode,
            NationalCode = parsedNationalCode,
            OrganGarrisonId = organGarrisonId,
            PasswordIsChanged = false,
            IsEnabled = true
        };

        var result = base.Create(user);
        if (result.Status)
            result.Message = "حساب کاربری رزرو غذا با موفقیت ایجاد شد.";

        return result;
    }

    private BaseResult EnsureReservationRole()
    {
        var role = UOW.Roles.FirstOrDefault(x =>
            x.IsDeleted == false &&
            x.Title == ReservationRoleTitle);

        if (role == null)
        {
            role = new Role
            {
                Title = ReservationRoleTitle,
                Description = "نقش خودکار پرسنل برای مشاهده و رزرو غذا",
                IsEnabled = true,
                IsDeleted = false,
                CreateDate = DateTime.Now
            };

            UOW.Roles.Add(role);
            if (!UOW.Commit())
                return new BaseResult(false, "ایجاد نقش رزرو غذا با خطا همراه بوده است.");
        }

        var allowedMenuIds = UOW.Menus
            .Get(x =>
                x.IsEnabled &&
                (
                    (x.Area == "admin" && x.Controller == "dashboard" && x.Action == "index") ||
                    (x.Area == "reservemanagment" && x.Controller == "reserve" && x.Action == "index") ||
                    (x.Area == "fooduser" && x.Controller == "myfood" && x.Action == "index")
                ))
            .Select(x => x.Id)
            .ToList();

        var currentMenuIds = UOW.RoleMenus
            .Get(x => x.RoleId == role.Id)
            .Select(x => x.MenuId)
            .ToList();

        foreach (var menuId in allowedMenuIds.Except(currentMenuIds))
        {
            UOW.RoleMenus.Add(new RoleMenu
            {
                RoleId = role.Id,
                MenuId = menuId
            });
        }

        if (UOW.Commit() == false)
            return new BaseResult(false, "ثبت دسترسی‌های نقش رزرو غذا با خطا همراه بوده است.");

        return new BaseResult(true, null, role.Id);
    }

    private static string Normalize(string value)
    {
        value = value?.Trim().ToEnglishNumber();
        return string.IsNullOrWhiteSpace(value) ? null : value;
    }
}
