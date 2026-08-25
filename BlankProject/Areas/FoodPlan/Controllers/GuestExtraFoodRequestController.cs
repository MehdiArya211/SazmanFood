using System;
using System.Linq;
using Microsoft.AspNetCore.Http;
using BLL.Interface;
using Domain.Enums;
using DTO.Entities;
using Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Food.Areas.FoodPlan.Controllers
{
    /// <summary>
    /// مدیریت درخواست غذای مهمان و سهمیه اضافه
    /// </summary>
    [Area("FoodPlan")]
    [UserAuthorize(Area: "FoodPlan", Controller: "GuestExtraFoodRequest", Action: "Index")]
    public class GuestExtraFoodRequestController : Controller
    {
        private readonly IGuestExtraFoodRequestManager manager;

        public GuestExtraFoodRequestController(IGuestExtraFoodRequestManager manager)
        {
            this.manager = manager ?? throw new ArgumentNullException(nameof(manager));
        }

        #region نمایش همه

        public IActionResult Index()
        {
            if (!manager.CanView())
                return AccessDenied();

            LoadAccess();
            LoadFormItems();

            return View();
        }

        [HttpPost]
        public IActionResult GetList(GuestExtraFoodRequestFilterDTO filters)
        {
            if (!manager.CanView())
                return AccessDenied();

            var data = manager.GetList(filters);
            var canApprove = manager.CanApprove();
            var canPrintGuest = manager.CanPrintGuest();

            var result = data.Select((x, index) => new
            {
                row = index + 1,
                id = x.Id,
                orgTitle = x.OrgTitle,
                mealTitle = x.MealTitle,
                personalTypeTitle = x.PersonalTypeTitle,
                yeganTypeTitle = x.YeganTypeTitle,
                fromDateFa = x.FromDateFa,
                toDateFa = x.ToDateFa,
                guestCount = x.GuestCount,
                extraQuotaCount = x.ExtraQuotaCount,
                status = x.Status,
                statusTitle = x.StatusTitle,
                attachmentCount = x.AttachmentCount,
                canEdit = x.Status == GuestExtraFoodRequestStatus.Draft,
                canDelete = x.Status == GuestExtraFoodRequestStatus.Draft,
                canSend = x.Status == GuestExtraFoodRequestStatus.Draft,
                canApprove = x.Status == GuestExtraFoodRequestStatus.Sent && canApprove,
                canPrintGuest = x.Status == GuestExtraFoodRequestStatus.Approved &&
                                x.GuestCount > 0 &&
                                canPrintGuest
            }).ToList();

            return Json(new { data = result });
        }

        #endregion

        #region ایجاد

        public IActionResult LoadCreateForm()
        {
            if (!manager.CanView())
                return AccessDenied();

            var canViewAll = manager.CanViewAllOrganizations();
            var currentOrgId = manager.GetCurrentUserOrganizationId();

            if (!canViewAll && (!currentOrgId.HasValue || currentOrgId.Value <= 0))
            {
                return BadRequest(new
                {
                    Status = false,
                    Message = "یگان کاربر مشخص نشده است."
                });
            }

            LoadFormItems();

            var model = new GuestExtraFoodRequestCreateDTO();

            if (!canViewAll)
                model.OrgId = currentOrgId;

            return PartialView("_Create", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(GuestExtraFoodRequestCreateDTO model)
        {
            try
            {
                if (!manager.CanView())
                    return AccessDenied();

                if (model == null)
                {
                    return Json(new
                    {
                        Status = false,
                        Message = "اطلاعات ارسالی معتبر نیست."
                    });
                }

                if (!manager.CanViewAllOrganizations())
                {
                    var currentOrgId = manager.GetCurrentUserOrganizationId();
                    if (!currentOrgId.HasValue || currentOrgId.Value <= 0)
                    {
                        return Json(new
                        {
                            Status = false,
                            Message = "یگان کاربر مشخص نشده است."
                        });
                    }

                    // OrgId برای کاربر یگانی از سشن تعیین می‌شود، نه از ورودی کاربر.
                    model.OrgId = currentOrgId.Value;
                    ModelState.Remove(nameof(model.OrgId));
                }

                // همان الگوی ModelState در UsersController
                if (ModelState.IsValid)
                {
                    var result = manager.CreateRequest(model);
                    return Json(result);
                }
                else
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage);

                    return Json(new
                    {
                        Status = false,
                        Message = string.Join("</br>", errors)
                    });
                }
            }
            catch
            {
                return Json(new
                {
                    Status = false,
                    Message = "ثبت درخواست با خطا همراه بود."
                });
            }
        }

        #endregion

        #region ویرایش

        public IActionResult LoadEditForm(long id)
        {
            if (!manager.CanView())
                return AccessDenied();

            var model = manager.GetEditDTO(id);
            if (model == null)
                return NotFound();

            LoadFormItems(
                model.OrgId,
                model.MealId,
                model.PersonalTypeId,
                model.YeganTypeId);

            return PartialView("_Edit", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(GuestExtraFoodRequestEditDTO model)
        {
            try
            {
                if (!manager.CanView())
                    return AccessDenied();

                if (model == null || model.Id <= 0)
                {
                    return Json(new
                    {
                        Status = false,
                        Message = "درخواست مورد نظر یافت نشد."
                    });
                }

                if (!manager.CanViewAllOrganizations())
                {
                    var currentOrgId = manager.GetCurrentUserOrganizationId();
                    if (!currentOrgId.HasValue || currentOrgId.Value <= 0)
                    {
                        return Json(new
                        {
                            Status = false,
                            Message = "یگان کاربر مشخص نشده است."
                        });
                    }

                    model.OrgId = currentOrgId.Value;
                    ModelState.Remove(nameof(model.OrgId));
                }

                // همان الگوی ModelState در UsersController
                if (ModelState.IsValid)
                {
                    var result = manager.EditRequest(model);
                    return Json(result);
                }
                else
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage);

                    return Json(new
                    {
                        Status = false,
                        Message = string.Join("</br>", errors)
                    });
                }
            }
            catch
            {
                return Json(new
                {
                    Status = false,
                    Message = "ویرایش درخواست با خطا همراه بود."
                });
            }
        }

        #endregion

        #region حذف

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(long id)
        {
            if (!manager.CanView())
                return AccessDenied();

            return Json(manager.DeleteRequest(id));
        }

        #endregion

        #region ارسال درخواست

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Send(long id)
        {
            if (!manager.CanView())
                return AccessDenied();

            return Json(manager.Send(id));
        }

        #endregion

        #region تایید درخواست

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Approve(long id)
        {
            if (!manager.CanApprove())
            {
                return Json(new
                {
                    Status = false,
                    Message = "شما مجوز تأیید درخواست را ندارید."
                });
            }

            return Json(manager.Approve(id));
        }

        #endregion

        #region مدیریت پیوست‌ها

        public IActionResult LoadAttachments(long id)
        {
            if (!manager.CanView())
                return AccessDenied();

            var attachments = manager.GetAttachments(id);
            ViewBag.RequestId = id;

            return PartialView("_Attachments", attachments);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddAttachment(long requestId, IFormFile file, string description)
        {
            if (!manager.CanView())
                return AccessDenied();

            return Json(manager.AddAttachment(requestId, file, description));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteAttachment(long id)
        {
            if (!manager.CanView())
                return AccessDenied();

            return Json(manager.DeleteAttachment(id));
        }

        public IActionResult DownloadAttachment(long id)
        {
            if (!manager.CanView())
                return AccessDenied();

            var attachment = manager.GetAttachment(id);
            if (attachment == null)
                return NotFound();

            return File(
                attachment.FileContent,
                attachment.ContentType,
                attachment.OriginalFileName);
        }

        #endregion

        #region چاپ توکن مهمان

        public IActionResult PrintGuestTokens(long id)
        {
            if (!manager.CanPrintGuest())
                return Forbid();

            var model = manager.GetForPrint(id);
            if (model == null)
                return NotFound();

            return View(model);
        }

        #endregion

        #region View helpers

        private void LoadAccess()
        {
            ViewBag.CanViewAllOrganizations = manager.CanViewAllOrganizations();
            ViewBag.CanApprove = manager.CanApprove();
            ViewBag.CanPrintGuest = manager.CanPrintGuest();
        }

        private void LoadFormItems(
            long? orgId = null,
            long? mealId = null,
            long? personalTypeId = null,
            long? yeganTypeId = null)
        {
            ViewBag.Organizations = new SelectList(
                manager.GetOrganizationSelectList(),
                "Id",
                "Title",
                orgId);

            ViewBag.Meals = new SelectList(
                manager.GetMealSelectList(),
                "Id",
                "Title",
                mealId);

            ViewBag.PersonalTypes = new SelectList(
                manager.GetPersonalTypeSelectList(),
                "Id",
                "Title",
                personalTypeId);

            ViewBag.YeganTypes = new SelectList(
                manager.GetYeganTypeSelectList(),
                "Id",
                "Title",
                yeganTypeId);
        }

        private IActionResult AccessDenied(
            string message = "شما مجوز انجام این عملیات را ندارید.")
        {
            return StatusCode(
                StatusCodes.Status403Forbidden,
                new
                {
                    Status = false,
                    Message = message
                });
        }

        #endregion
    }
}
