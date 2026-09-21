using Domain.Constants;
using Food.Areas.FoodPlan.Models;
using Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Services.SessionServices;

namespace Food.Areas.FoodPlan.Controllers;

[Area("FoodPlan")]
public class KitchenCookingStatisticsController : Controller
{
    private readonly ApplicationContext context;
    private readonly ISession session;

    public KitchenCookingStatisticsController(
        ApplicationContext context,
        IHttpContextAccessor httpContextAccessor)
    {
        this.context = context;
        session = httpContextAccessor.HttpContext.Session;
    }

    [HttpGet]
    public async Task<IActionResult> Index(
        DateTime? fromDate,
        DateTime? toDate,
        long? kitchenId,
        long? mealId)
    {
        var user = session.GetUser();
        if (user?.IsEnabled != true || user.RoleId != RoleConstant.Admin)
            return Forbid();

        var from = (fromDate ?? DateTime.Today).Date;
        var to = (toDate ?? from.AddDays(6)).Date;
        if (to < from)
            (from, to) = (to, from);

        // از ایجاد گزارش‌های ناخواسته و بسیار سنگین جلوگیری می‌کند.
        if ((to - from).TotalDays > 92)
        {
            ModelState.AddModelError(string.Empty, "بازه گزارش نمی‌تواند بیشتر از ۹۳ روز باشد.");
            to = from.AddDays(92);
        }

        var kitchens = await context.Kitchens
            .AsNoTracking()
            .Where(x => x.IsDeleted != true && x.IsActive == true)
            .OrderBy(x => x.OrgTitle)
            .ThenBy(x => x.Title)
            .Select(x => new
            {
                x.Id,
                x.Title,
                x.OrgId,
                x.OrgTitle,
                CookingCapacity = x.CookingCapacity ?? 0
            })
            .ToListAsync();

        var allocations = await context.QoutaAllocations
            .AsNoTracking()
            .Where(x => x.IsDeleted != true &&
                        x.QoutaAllocationDate >= from &&
                        x.QoutaAllocationDate < to.AddDays(1) &&
                        (!mealId.HasValue || x.MealId == mealId.Value))
            .Select(x => new
            {
                x.OrgId,
                Date = x.QoutaAllocationDate.Date,
                x.MealId,
                MealTitle = x.MealTitle ?? x.Meal.Title,
                QuotaCount = x.OfficerCapacity + x.SoldierCapacity +
                             x.GuestCapacity + x.ManagementTokenCapacity,
                ReservedCount = x.QoutaPerson.Count(p => p.IsDeleted != true),
                DeliveredCount = x.QoutaPerson.Count(p => p.IsDeleted != true && p.IsDelivered == true)
            })
            .ToListAsync();

        var rows = (
            from kitchen in kitchens
            where !kitchenId.HasValue || kitchen.Id == kitchenId.Value
            join allocation in allocations
                on (long?)kitchen.OrgId equals allocation.OrgId
            group allocation by new
            {
                kitchen.Id,
                KitchenTitle = kitchen.Title,
                kitchen.OrgTitle,
                kitchen.CookingCapacity,
                allocation.Date,
                allocation.MealId,
                allocation.MealTitle
            }
            into grouped
            orderby grouped.Key.Date, grouped.Key.KitchenTitle, grouped.Key.MealTitle
            select new KitchenCookingStatisticsRow
            {
                KitchenId = grouped.Key.Id,
                KitchenTitle = grouped.Key.KitchenTitle,
                OrgTitle = grouped.Key.OrgTitle,
                Date = grouped.Key.Date,
                MealId = grouped.Key.MealId,
                MealTitle = grouped.Key.MealTitle ?? "-",
                CookingCapacity = grouped.Key.CookingCapacity,
                QuotaCount = grouped.Sum(x => x.QuotaCount),
                ReservedCount = grouped.Sum(x => x.ReservedCount),
                DeliveredCount = grouped.Sum(x => x.DeliveredCount)
            }).ToList();

        var meals = await context.Meal
            .AsNoTracking()
            .OrderBy(x => x.SortName)
            .ThenBy(x => x.Title)
            .Select(x => new KitchenCookingFilterItem { Id = x.Id, Title = x.Title })
            .ToListAsync();

        return View(new KitchenCookingStatisticsViewModel
        {
            FromDate = from,
            ToDate = to,
            KitchenId = kitchenId,
            MealId = mealId,
            Kitchens = kitchens.Select(x => new KitchenCookingFilterItem
            {
                Id = x.Id,
                Title = $"{x.Title} - {x.OrgTitle}"
            }).ToList(),
            Meals = meals,
            Rows = rows
        });
    }
}
