using Domain.Enums;
using Utilities.Extentions;

namespace DTO.Entities
{
    public class GuestExtraFoodRequestDTO
    {
        public long Id { get; set; }

        public int OrgId { get; set; }

        public string OrgTitle { get; set; }

        public long MealId { get; set; }

        public string MealTitle { get; set; }

        public long PersonalTypeId { get; set; }

        public string PersonalTypeTitle { get; set; }

        public long YeganTypeId { get; set; }

        public string YeganTypeTitle { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public int GuestCount { get; set; }

        public int ExtraQuotaCount { get; set; }

        public GuestExtraFoodRequestStatus Status { get; set; }

        public int AttachmentCount { get; set; }

        public string FromDateFa =>
            FromDate.ToPersianDateTime().ToString();

        public string ToDateFa =>
            ToDate.ToPersianDateTime().ToString();

        public string StatusTitle =>
            Status switch
            {
                GuestExtraFoodRequestStatus.Draft =>
                    "ثبت اولیه",

                GuestExtraFoodRequestStatus.Sent =>
                    "ارسال شده",

                GuestExtraFoodRequestStatus.Approved =>
                    "تأیید شده",

                _ => "نامشخص"
            };
    }

    public class GuestExtraFoodRequestFilterDTO
    {
        public int? OrgId { get; set; }

        public long? MealId { get; set; }

        public long? PersonalTypeId { get; set; }

        public long? YeganTypeId { get; set; }

        public DateTime? FromDate { get; set; }

        public DateTime? ToDate { get; set; }

        public GuestExtraFoodRequestStatus? Status { get; set; }
    }

    public class GuestExtraFoodRequestAttachmentDTO
    {
        public long Id { get; set; }

        public long RequestId { get; set; }

        public string OriginalFileName { get; set; }

        public string Description { get; set; }

        public long FileSize { get; set; }

        public string UploaderFullName { get; set; }

        public DateTime UploadDate { get; set; }

        public bool CanDelete { get; set; }

        public string UploadDateFa =>
            UploadDate.ToPersianDateTime().ToString();

        public string FileSizeTitle =>
            FileSize < 1024
                ? $"{FileSize} B"
                : FileSize < 1024 * 1024
                    ? $"{FileSize / 1024D:0.##} KB"
                    : $"{FileSize / (1024D * 1024D):0.##} MB";
    }
}