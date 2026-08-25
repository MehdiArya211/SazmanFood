using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BLL.Interface;
using Domain.Entities;
using Domain.Enums;
using DTO.Base;
using DTO.Entities;
using DTO.User;
using Infrastructure.Data;
using ITOWebApiClient;
using Microsoft.AspNetCore.Http;
using Services.SessionServices;

namespace BLL
{
    /// <summary>
    /// مدیریت منطق درخواست غذای مهمان و سهمیه اضافه
    /// </summary>
    public class GuestExtraFoodRequestManager
        : Manager<GuestExtraFoodRequest, ApplicationContext>, IGuestExtraFoodRequestManager
    {
        #region Constants

        private const string RegistrarRole = "ثبت‌کننده آمار یگان";
        private const string ApproverRole = "تایید کننده آمار یگان";
        private const string SupportRole = "رکن 4 پشتیبانی قرارگاه";
        private const string OfficeRole = "اداری یگان";

        private const long MaxFileSize = 5 * 1024 * 1024;

        private static readonly string[] AllowedExtensions =
        {
            ".pdf",
            ".jpg",
            ".jpeg",
            ".png",
            ".doc",
            ".docx"
        };

        #endregion

        #region Fields

        private readonly ISession Session;
        private readonly IMealManager mealManager;
        private readonly IFoodSourceManager foodSourceManager;
        private readonly IWebApiManager webApiManager;
        private readonly ApiTokenCacheClient apiTokenClient;
        private readonly string accessToken;

        #endregion

        #region Constructor

        public GuestExtraFoodRequestManager(
            DbContexts contexts,
            IHttpContextAccessor httpContextAccessor,
            IMealManager mealManager,
            IFoodSourceManager foodSourceManager,
            IWebApiManager webApiManager,
            ApiTokenCacheClient apiTokenClient)
            : base(contexts, httpContextAccessor)
        {
            if (httpContextAccessor == null)
                throw new ArgumentNullException(nameof(httpContextAccessor));

            this.mealManager = mealManager ?? throw new ArgumentNullException(nameof(mealManager));
            this.foodSourceManager = foodSourceManager ?? throw new ArgumentNullException(nameof(foodSourceManager));
            this.webApiManager = webApiManager ?? throw new ArgumentNullException(nameof(webApiManager));
            this.apiTokenClient = apiTokenClient ?? throw new ArgumentNullException(nameof(apiTokenClient));

            Session = httpContextAccessor.HttpContext?.Session;

            accessToken = this.apiTokenClient.GetApiToken(
                CustomSettings.Instance.ClientId,
                CustomSettings.Instance.Scope,
                CustomSettings.Instance.ClientSecret,
                CustomSettings.Instance.ROPC_UserName,
                CustomSettings.Instance.ROPC_Password
            ).GetAwaiter().GetResult();
        }

        #endregion

        #region Access

        public bool CanView()
        {
            return CanViewAllOrganizations() || IsUnitUser();
        }

        public bool CanViewAllOrganizations()
        {
            var user = GetCurrentUser();

            return user != null &&
                   user.IsEnabled &&
                   string.Equals(
                       user.Role?.Trim(),
                       SupportRole,
                       StringComparison.OrdinalIgnoreCase);
        }

        public bool CanApprove()
        {
            // رفتار فعلی سیستم حفظ شده است:
            // تایید نهایی فقط برای نقش پشتیبانی قرارگاه مجاز است.
            return CanViewAllOrganizations();
        }

        public bool CanPrintGuest()
        {
            var user = GetCurrentUser();

            return user != null &&
                   user.IsEnabled &&
                   string.Equals(
                       user.Role?.Trim(),
                       OfficeRole,
                       StringComparison.OrdinalIgnoreCase);
        }

        public int? GetCurrentUserOrganizationId()
        {
            var user = GetCurrentUser();
            return user != null && user.OmdOrgId > 0
                ? user.OmdOrgId
                : null;
        }

        private bool IsUnitUser()
        {
            var user = GetCurrentUser();

            if (user == null || !user.IsEnabled)
                return false;

            var role = user.Role?.Trim();

            return string.Equals(role, RegistrarRole, StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(role, ApproverRole, StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(role, OfficeRole, StringComparison.OrdinalIgnoreCase);
        }

        private UserSessionDTO GetCurrentUser()
        {
            return Session?.GetUser();
        }

        #endregion

        #region Lookup data

        public IList<SelectListDTO> GetOrganizationSelectList()
        {
            var user = GetCurrentUser();
            if (user == null || !CanView())
                return new List<SelectListDTO>();

            var organizations = GetOrganizations();

            if (CanViewAllOrganizations())
                return organizations;

            if (user.OmdOrgId <= 0)
                return new List<SelectListDTO>();

            return organizations
                .Where(x => x.Id == user.OmdOrgId)
                .ToList();
        }

        public IList<SelectListDTO> GetMealSelectList()
        {
            return mealManager.GetSelectListDTO().ToList();
        }

        public IList<SelectListDTO> GetPersonalTypeSelectList()
        {
            return foodSourceManager.GetSelectListPersonalTypeDTO().ToList();
        }

        public IList<SelectListDTO> GetYeganTypeSelectList()
        {
            return foodSourceManager.GetSelectListYeganTypeDTO().ToList();
        }

        private List<SelectListDTO> GetOrganizations()
        {
            return webApiManager
                .GetListOrganInfoV1(accessToken)
                .Select(x => new SelectListDTO
                {
                    Id = x.Id,
                    Title = x.UnitTitle
                })
                .ToList();
        }

        private string GetOrganizationTitle(long orgId)
        {
            if (orgId <= 0)
                return null;

            return webApiManager
                .GetListOrganInfoV1(accessToken)
                .Where(x => x.Id == orgId)
                .Select(x => x.UnitTitle)
                .FirstOrDefault();
        }

        #endregion

        #region List

        public List<GuestExtraFoodRequestDTO> GetList(
            GuestExtraFoodRequestFilterDTO filters)
        {
            var user = GetCurrentUser();
            if (user == null || !CanView())
                return new List<GuestExtraFoodRequestDTO>();

            filters ??= new GuestExtraFoodRequestFilterDTO();
            var canViewAll = CanViewAllOrganizations();

            var query = UOW.GuestExtraFoodRequest
                .GetAll()
                .Where(x => x.IsDeleted != true);

            if (!canViewAll)
            {
                if (user.OmdOrgId <= 0)
                    return new List<GuestExtraFoodRequestDTO>();

                query = query.Where(x => x.OrgId == user.OmdOrgId);
            }
            else if (filters.OrgId.HasValue)
            {
                query = query.Where(x => x.OrgId == filters.OrgId.Value);
            }

            if (filters.MealId.HasValue)
                query = query.Where(x => x.MealId == filters.MealId.Value);

            if (filters.PersonalTypeId.HasValue)
                query = query.Where(x => x.PersonalTypeId == filters.PersonalTypeId.Value);

            if (filters.YeganTypeId.HasValue)
                query = query.Where(x => x.YeganTypeId == filters.YeganTypeId.Value);

            if (filters.Status.HasValue)
                query = query.Where(x => x.Status == filters.Status.Value);

            if (filters.FromDate.HasValue)
            {
                var fromDate = filters.FromDate.Value.Date;
                query = query.Where(x => x.ToDate >= fromDate);
            }

            if (filters.ToDate.HasValue)
            {
                var toDate = filters.ToDate.Value.Date;
                query = query.Where(x => x.FromDate <= toDate);
            }

            var meals = UOW.Meal.GetAll().ToList();
            var personalTypes = UOW.PersonalType.GetAll().ToList();
            var yeganTypes = UOW.YeganType.GetAll().ToList();
            var attachments = UOW.GuestExtraFoodRequestAttachment
                .GetAll()
                .Where(x => x.IsDeleted != true)
                .ToList();

            var requests = query
                .OrderByDescending(x => x.Id)
                .ToList();

            return requests.Select(x => new GuestExtraFoodRequestDTO
            {
                Id = x.Id,
                OrgId = x.OrgId,
                OrgTitle = x.OrgTitle,
                MealId = x.MealId,
                MealTitle = meals.FirstOrDefault(m => m.Id == x.MealId)?.Title,
                PersonalTypeId = x.PersonalTypeId,
                PersonalTypeTitle = personalTypes.FirstOrDefault(p => p.Id == x.PersonalTypeId)?.Title,
                YeganTypeId = x.YeganTypeId,
                YeganTypeTitle = yeganTypes.FirstOrDefault(y => y.Id == x.YeganTypeId)?.Title,
                FromDate = x.FromDate,
                ToDate = x.ToDate,
                GuestCount = x.GuestCount,
                ExtraQuotaCount = x.ExtraQuotaCount,
                Status = x.Status,
                AttachmentCount = attachments.Count(a => a.GuestExtraFoodRequestId == x.Id),
                ReturnerFullName = x.ReturnerFullName,
                ReturnDate = x.ReturnDate,
                ReturnReason = x.ReturnReason,
                CancelerFullName = x.CancelerFullName,
                CancelDate = x.CancelDate,
                CancelReason = x.CancelReason
            }).ToList();
        }

        #endregion

        #region Edit DTO

        public GuestExtraFoodRequestEditDTO GetEditDTO(long id)
        {
            var request = GetAuthorizedRequest(id);

            if (request == null ||
                (request.Status != GuestExtraFoodRequestStatus.Draft &&
                 request.Status != GuestExtraFoodRequestStatus.Returned))
                return null;

            return new GuestExtraFoodRequestEditDTO
            {
                Id = request.Id,
                OrgId = request.OrgId,
                MealId = request.MealId,
                PersonalTypeId = request.PersonalTypeId,
                YeganTypeId = request.YeganTypeId,
                FromDate = request.FromDate,
                ToDate = request.ToDate,
                GuestCount = request.GuestCount,
                ExtraQuotaCount = request.ExtraQuotaCount
            };
        }

        #endregion

        #region Create

        public BaseResult CreateRequest(GuestExtraFoodRequestCreateDTO model)
        {
            var user = GetCurrentUser();
            var canViewAll = CanViewAllOrganizations();

            var validation = ValidateModel(model, user, canViewAll);
            if (!validation.Status)
                return validation;

            var orgId = canViewAll
                ? model.OrgId.Value
                : user.OmdOrgId;

            var orgTitle = GetOrganizationTitle(orgId);
            if (string.IsNullOrWhiteSpace(orgTitle))
                return new BaseResult(false, "عنوان یگان یافت نشد.");

            var now = DateTime.Now;

            var entity = new GuestExtraFoodRequest
            {
                OrgId = orgId,
                OrgTitle = orgTitle.Trim(),
                MealId = model.MealId.Value,
                PersonalTypeId = model.PersonalTypeId.Value,
                YeganTypeId = model.YeganTypeId.Value,
                FromDate = model.FromDate.Value.Date,
                ToDate = model.ToDate.Value.Date,
                GuestCount = model.GuestCount,
                ExtraQuotaCount = model.ExtraQuotaCount,
                Status = GuestExtraFoodRequestStatus.Draft,
                CreatorId = user.Id,
                CreatorFullName = user.FullName,
                RequestCreateDate = now,
                RegUserId = user.Id,
                RegDate = now,
                CreateDate = now,
                IsDeleted = false
            };

            UOW.GuestExtraFoodRequest.Add(entity);

            var success = UOW.Commit();

            return new BaseResult(
                success,
                success
                    ? "درخواست با موفقیت ثبت شد."
                    : "ثبت درخواست با خطا همراه بود.",
                success ? entity.Id : null);
        }

        #endregion

        #region Edit

        public BaseResult EditRequest(GuestExtraFoodRequestEditDTO model)
        {
            var user = GetCurrentUser();
            var canViewAll = CanViewAllOrganizations();

            var validation = ValidateModel(model, user, canViewAll);
            if (!validation.Status)
                return validation;

            var entity = GetAuthorizedRequest(model.Id);
            if (entity == null)
                return new BaseResult(false, "درخواست یافت نشد یا شما به آن دسترسی ندارید.");

            if (entity.Status != GuestExtraFoodRequestStatus.Draft &&
                entity.Status != GuestExtraFoodRequestStatus.Returned)
                return new BaseResult(false, "فقط درخواست ثبت اولیه یا عودت‌شده قابل ویرایش است.");

            var orgId = canViewAll
                ? model.OrgId.Value
                : user.OmdOrgId;

            var orgTitle = GetOrganizationTitle(orgId);
            if (string.IsNullOrWhiteSpace(orgTitle))
                return new BaseResult(false, "عنوان یگان یافت نشد.");

            entity.OrgId = orgId;
            entity.OrgTitle = orgTitle.Trim();
            entity.MealId = model.MealId.Value;
            entity.PersonalTypeId = model.PersonalTypeId.Value;
            entity.YeganTypeId = model.YeganTypeId.Value;
            entity.FromDate = model.FromDate.Value.Date;
            entity.ToDate = model.ToDate.Value.Date;
            entity.GuestCount = model.GuestCount;
            entity.ExtraQuotaCount = model.ExtraQuotaCount;
            entity.Status = GuestExtraFoodRequestStatus.Draft;
            entity.LastEditUserId = user.Id;
            entity.LastEditDate = DateTime.Now;

            UOW.GuestExtraFoodRequest.Update(entity);

            var success = UOW.Commit();

            return new BaseResult(
                success,
                success
                    ? "درخواست با موفقیت ویرایش شد."
                    : "ویرایش درخواست با خطا همراه بود.");
        }

        #endregion

        #region Delete

        public BaseResult DeleteRequest(long id)
        {
            var user = GetCurrentUser();
            if (user == null || !CanView())
                return new BaseResult(false, "اطلاعات کاربر یافت نشد یا دسترسی مجاز نیست.");

            var entity = GetAuthorizedRequest(id);
            if (entity == null)
                return new BaseResult(false, "درخواست یافت نشد یا شما به آن دسترسی ندارید.");

            if (entity.Status != GuestExtraFoodRequestStatus.Draft &&
                entity.Status != GuestExtraFoodRequestStatus.Returned)
                return new BaseResult(false, "فقط درخواست ثبت اولیه یا عودت‌شده قابل حذف است.");

            entity.IsDeleted = true;
            entity.LastEditUserId = user.Id;
            entity.LastEditDate = DateTime.Now;

            UOW.GuestExtraFoodRequest.Update(entity);

            var success = UOW.Commit();

            return new BaseResult(
                success,
                success
                    ? "درخواست با موفقیت حذف شد."
                    : "حذف درخواست با خطا همراه بود.");
        }

        #endregion

        #region Send

        public BaseResult Send(long id)
        {
            var user = GetCurrentUser();
            if (user == null || !CanView())
                return new BaseResult(false, "اطلاعات کاربر یافت نشد یا دسترسی مجاز نیست.");

            var entity = GetAuthorizedRequest(id);
            if (entity == null)
                return new BaseResult(false, "درخواست یافت نشد یا شما به آن دسترسی ندارید.");

            if (entity.Status != GuestExtraFoodRequestStatus.Draft)
                return new BaseResult(false, "فقط درخواست ثبت اولیه قابل ارسال است.");

            var now = DateTime.Now;

            entity.Status = GuestExtraFoodRequestStatus.Sent;
            entity.SenderId = user.Id;
            entity.SenderFullName = user.FullName;
            entity.SendDate = now;
            entity.LastEditUserId = user.Id;
            entity.LastEditDate = now;

            UOW.GuestExtraFoodRequest.Update(entity);

            var success = UOW.Commit();

            return new BaseResult(
                success,
                success
                    ? "درخواست با موفقیت ارسال شد."
                    : "ارسال درخواست با خطا همراه بود.");
        }

        #endregion

        #region Approve

        public BaseResult Approve(long id)
        {
            if (!CanApprove())
                return new BaseResult(false, "شما مجوز تأیید درخواست را ندارید.");

            var user = GetCurrentUser();
            if (user == null)
                return new BaseResult(false, "اطلاعات کاربر یافت نشد.");

            var entity = GetAuthorizedRequest(id);
            if (entity == null)
                return new BaseResult(false, "درخواست یافت نشد یا شما به آن دسترسی ندارید.");

            if (entity.Status != GuestExtraFoodRequestStatus.Sent)
                return new BaseResult(false, "فقط درخواست ارسال‌شده قابل تأیید است.");

            var now = DateTime.Now;

            entity.Status = GuestExtraFoodRequestStatus.Approved;
            entity.ApproverId = user.Id;
            entity.ApproverFullName = user.FullName;
            entity.ApproveDate = now;
            entity.LastEditUserId = user.Id;
            entity.LastEditDate = now;

            UOW.GuestExtraFoodRequest.Update(entity);

            var success = UOW.Commit();

            return new BaseResult(
                success,
                success
                    ? "درخواست با موفقیت تأیید شد."
                    : "تأیید درخواست با خطا همراه بود.");
        }

        #endregion

        #region Return

        /// <summary>
        /// عودت درخواست ارسال‌شده برای اصلاح
        /// </summary>
        public BaseResult Return(long id, string reason)
        {
            if (!CanApprove())
                return new BaseResult(false, "شما مجوز عودت درخواست را ندارید.");

            var user = GetCurrentUser();

            if (user == null)
                return new BaseResult(false, "اطلاعات کاربر یافت نشد.");

            reason = reason?.Trim();

            if (string.IsNullOrWhiteSpace(reason))
                return new BaseResult(false, "ثبت توضیحات و دلیل عودت الزامی است.");

            if (reason.Length > 1000)
                return new BaseResult(false, "توضیحات عودت نمی‌تواند بیشتر از 1000 کاراکتر باشد.");

            var entity = GetAuthorizedRequest(id);

            if (entity == null)
                return new BaseResult(false, "درخواست یافت نشد یا شما به آن دسترسی ندارید.");

            if (entity.Status != GuestExtraFoodRequestStatus.Sent)
                return new BaseResult(false, "فقط درخواست ارسال‌شده قابل عودت است.");

            var now = DateTime.Now;

            entity.Status = GuestExtraFoodRequestStatus.Returned;
            entity.ReturnerId = user.Id;
            entity.ReturnerFullName = user.FullName;
            entity.ReturnDate = now;
            entity.ReturnReason = reason;
            entity.LastEditUserId = user.Id;
            entity.LastEditDate = now;

            UOW.GuestExtraFoodRequest.Update(entity);

            var success = UOW.Commit();

            return new BaseResult(
                success,
                success
                    ? "درخواست برای اصلاح عودت داده شد."
                    : "عودت درخواست با خطا همراه بود.");
        }

        #endregion

        #region Cancel

        /// <summary>
        /// لغو درخواست ارسال‌شده با حفظ سابقه
        /// </summary>
        public BaseResult Cancel(long id, string reason)
        {
            if (!CanApprove())
                return new BaseResult(false, "شما مجوز لغو درخواست را ندارید.");

            var user = GetCurrentUser();

            if (user == null)
                return new BaseResult(false, "اطلاعات کاربر یافت نشد.");

            reason = reason?.Trim();

            if (string.IsNullOrWhiteSpace(reason))
                return new BaseResult(false, "ثبت توضیحات و دلیل لغو الزامی است.");

            if (reason.Length > 1000)
                return new BaseResult(false, "توضیحات لغو نمی‌تواند بیشتر از 1000 کاراکتر باشد.");

            var entity = GetAuthorizedRequest(id);

            if (entity == null)
                return new BaseResult(false, "درخواست یافت نشد یا شما به آن دسترسی ندارید.");

            if (entity.Status != GuestExtraFoodRequestStatus.Sent)
                return new BaseResult(false, "فقط درخواست ارسال‌شده قابل لغو است.");

            var now = DateTime.Now;

            entity.Status = GuestExtraFoodRequestStatus.Canceled;
            entity.CancelerId = user.Id;
            entity.CancelerFullName = user.FullName;
            entity.CancelDate = now;
            entity.CancelReason = reason;
            entity.LastEditUserId = user.Id;
            entity.LastEditDate = now;

            UOW.GuestExtraFoodRequest.Update(entity);

            var success = UOW.Commit();

            return new BaseResult(
                success,
                success
                    ? "درخواست با موفقیت لغو شد."
                    : "لغو درخواست با خطا همراه بود.");
        }

        #endregion

        #region Attachments

        public List<GuestExtraFoodRequestAttachmentDTO> GetAttachments(long requestId)
        {
            var request = GetAuthorizedRequest(requestId);
            if (request == null)
                return new List<GuestExtraFoodRequestAttachmentDTO>();

            return UOW.GuestExtraFoodRequestAttachment
                .GetByRequestId(requestId)
                .Select(x => new GuestExtraFoodRequestAttachmentDTO
                {
                    Id = x.Id,
                    RequestId = requestId,
                    OriginalFileName = x.OriginalFileName,
                    Description = x.Description,
                    FileSize = x.FileSize,
                    UploaderFullName = x.UploaderFullName,
                    UploadDate = x.UploadDate,
                    CanDelete = request.Status == GuestExtraFoodRequestStatus.Draft ||
                                request.Status == GuestExtraFoodRequestStatus.Returned
                })
                .ToList();
        }

        public BaseResult AddAttachment(
            long requestId,
            IFormFile file,
            string description)
        {
            var user = GetCurrentUser();
            if (user == null || !CanView())
                return new BaseResult(false, "اطلاعات کاربر یافت نشد یا دسترسی مجاز نیست.");

            var request = GetAuthorizedRequest(requestId);
            if (request == null)
                return new BaseResult(false, "درخواست یافت نشد یا شما به آن دسترسی ندارید.");

            if (request.Status != GuestExtraFoodRequestStatus.Draft &&
                request.Status != GuestExtraFoodRequestStatus.Returned)
                return new BaseResult(false, "پیوست فقط در وضعیت ثبت اولیه یا عودت‌شده قابل تغییر است.");

            var fileValidation = ValidateAttachment(file);
            if (!fileValidation.Status)
                return fileValidation;

            byte[] content;

            using (var stream = new MemoryStream())
            {
                file.CopyTo(stream);
                content = stream.ToArray();
            }

            var now = DateTime.Now;

            var entity = new GuestExtraFoodRequestAttachment
            {
                GuestExtraFoodRequestId = requestId,
                OriginalFileName = Path.GetFileName(file.FileName),
                ContentType = string.IsNullOrWhiteSpace(file.ContentType)
                    ? "application/octet-stream"
                    : file.ContentType,
                FileSize = file.Length,
                FileContent = content,
                Description = description?.Trim(),
                UploaderId = user.Id,
                UploaderFullName = user.FullName,
                UploadDate = now,
                RegUserId = user.Id,
                RegDate = now,
                IsDeleted = false
            };

            UOW.GuestExtraFoodRequestAttachment.Add(entity);

            var success = UOW.Commit();

            return new BaseResult(
                success,
                success
                    ? "پیوست با موفقیت ثبت شد."
                    : "ثبت پیوست با خطا همراه بود.");
        }

        public BaseResult DeleteAttachment(long id)
        {
            var user = GetCurrentUser();
            if (user == null || !CanView())
                return new BaseResult(false, "اطلاعات کاربر یافت نشد یا دسترسی مجاز نیست.");

            var attachment = UOW.GuestExtraFoodRequestAttachment.FirstOrDefault(x =>
                x.Id == id &&
                x.IsDeleted != true);

            if (attachment == null)
                return new BaseResult(false, "پیوست یافت نشد.");

            var request = GetAuthorizedRequest(attachment.GuestExtraFoodRequestId);
            if (request == null)
                return new BaseResult(false, "درخواست یافت نشد یا شما به آن دسترسی ندارید.");

            if (request.Status != GuestExtraFoodRequestStatus.Draft &&
                request.Status != GuestExtraFoodRequestStatus.Returned)
                return new BaseResult(false, "پیوست فقط در وضعیت ثبت اولیه یا عودت‌شده قابل حذف است.");

            attachment.IsDeleted = true;
            attachment.LastEditUserId = user.Id;
            attachment.LastEditDate = DateTime.Now;

            UOW.GuestExtraFoodRequestAttachment.Update(attachment);

            var success = UOW.Commit();

            return new BaseResult(
                success,
                success
                    ? "پیوست با موفقیت حذف شد."
                    : "حذف پیوست با خطا همراه بود.");
        }

        public GuestExtraFoodRequestAttachment GetAttachment(long id)
        {
            var attachment = UOW.GuestExtraFoodRequestAttachment.FirstOrDefault(x =>
                x.Id == id &&
                x.IsDeleted != true);

            if (attachment == null)
                return null;

            var request = GetAuthorizedRequest(attachment.GuestExtraFoodRequestId);
            return request == null ? null : attachment;
        }

        #endregion

        #region Print

        public GuestExtraFoodRequest GetForPrint(long id)
        {
            if (!CanPrintGuest())
                return null;

            var user = GetCurrentUser();
            if (user == null)
                return null;

            var entity = UOW.GuestExtraFoodRequest.GetRequest(id);

            if (entity == null ||
                entity.OrgId != user.OmdOrgId ||
                entity.Status != GuestExtraFoodRequestStatus.Approved ||
                entity.GuestCount <= 0)
            {
                return null;
            }

            var today = DateTime.Now.Date;

            return entity.FromDate <= today && entity.ToDate >= today
                ? entity
                : null;
        }

        #endregion

        #region Private access helpers

        private GuestExtraFoodRequest GetAuthorizedRequest(long id)
        {
            var user = GetCurrentUser();
            if (user == null || !CanView())
                return null;

            var request = UOW.GuestExtraFoodRequest.GetRequest(id);
            if (request == null)
                return null;

            if (!CanViewAllOrganizations() && request.OrgId != user.OmdOrgId)
                return null;

            return request;
        }

        #endregion

        #region Request validation

        private BaseResult ValidateModel(
            GuestExtraFoodRequestCreateDTO model,
            UserSessionDTO user,
            bool canViewAll)
        {
            if (user == null || !CanView())
                return new BaseResult(false, "اطلاعات کاربر یافت نشد یا دسترسی مجاز نیست.");

            if (model == null)
                return new BaseResult(false, "اطلاعات معتبر نیست.");

            if (!canViewAll)
            {
                if (user.OmdOrgId <= 0)
                    return new BaseResult(false, "یگان کاربر مشخص نشده است.");

                // به OrgId ارسال‌شده از Client اعتماد نمی‌کنیم.
                model.OrgId = user.OmdOrgId;
            }

            if (!model.OrgId.HasValue || model.OrgId.Value <= 0)
                return new BaseResult(false, "یگان الزامی است.");

            if (!model.MealId.HasValue ||
                !model.PersonalTypeId.HasValue ||
                !model.YeganTypeId.HasValue)
            {
                return new BaseResult(false, "وعده، نوع پرسنل و نوع خدمت الزامی است.");
            }

            if (!model.FromDate.HasValue || !model.ToDate.HasValue)
                return new BaseResult(false, "بازه زمانی الزامی است.");

            if (model.FromDate.Value.Date > model.ToDate.Value.Date)
                return new BaseResult(false, "تاریخ شروع نمی‌تواند بعد از تاریخ پایان باشد.");

            if (model.GuestCount <= 0 && model.ExtraQuotaCount <= 0)
                return new BaseResult(false, "حداقل یکی از تعدادها باید بیشتر از صفر باشد.");

            return new BaseResult(true, null);
        }

        #endregion

        #region Attachment validation

        private BaseResult ValidateAttachment(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return new BaseResult(false, "فایل انتخاب نشده است.");

            if (file.Length > MaxFileSize)
                return new BaseResult(false, "حداکثر حجم فایل 5 مگابایت است.");

            var extension = Path.GetExtension(file.FileName)?.ToLowerInvariant();

            if (string.IsNullOrWhiteSpace(extension) ||
                !AllowedExtensions.Contains(extension))
            {
                return new BaseResult(false, "فرمت فایل مجاز نیست.");
            }

            return new BaseResult(true, null);
        }

        #endregion
    }
}
