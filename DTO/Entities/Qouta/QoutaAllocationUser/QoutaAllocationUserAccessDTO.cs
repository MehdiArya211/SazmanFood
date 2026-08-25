namespace DTO.Entities
{
    /// <summary>
    /// اطلاعات دسترسی کاربر برای مشاهده سهمیه بندی
    /// </summary>
    public class QoutaAllocationUserAccessDTO
    {
        public long UserId { get; set; }

        public string RoleTitle { get; set; }

        public long? OrganGarrisonId { get; set; }

        public bool IsAdmin
        {
            get
            {
                return RoleTitle == "مدیر سیستم" ||
                       RoleTitle == "مدیر کل سیستم" ||
                       RoleTitle == "ادمین" ||
                       string.Equals(RoleTitle, "Admin", StringComparison.OrdinalIgnoreCase);
            }
        }

        public bool IsAdministrativeManager
        {
            get
            {
                return RoleTitle == "مدیر اداری";
            }
        }
    }
}