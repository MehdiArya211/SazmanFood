using Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;

namespace DTO.User
{
    /// <summary>
    /// ویرایش پرسنل
    /// </summary>
    public class UserEditDTO
    {

        [Display(Name = "شناسه")]
        public long Id { get; set; }



        [Display(Name = "نام و نام خانوادگی")]
        [Required(ErrorMessage = "{0} الزامی است.")]
        [MaxLength(50, ErrorMessage = "{0} حداکثر می تواند {1} کاراکتر باشد.")]
        public string Name { get; set; }



        [Display(Name = "نام کاربری")]
        [Required(ErrorMessage = "{0} الزامی است.")]
        [StringLength(30, ErrorMessage = "{0} حداکثر {1} کاراکتر باشد.")]
        [Remote("UsernameIsUnique", "CheckUnique", "Global",
            AdditionalFields = "Id",
            HttpMethod = "post",
            ErrorMessage = "نام کاربری قبلا استفاده شده است!")]
        public string Username { get; set; }



        [Display(Name = "تلفن")]
        [StringLength(11, MinimumLength = 11,
            ErrorMessage = "{0} باید {1} کاراکتر باشد.")]
        [RegularExpression(@"^0\d{10}",
            ErrorMessage = "تلفن با صفر شروع شود و حاوی عدد به طول 11 کاراکتر باشد.")]
        [Remote("MobileIsUnique", "CheckUnique", "Global",
            AdditionalFields = "Id",
            HttpMethod = "post",
            ErrorMessage = "تلفن قبلا استفاده شده است!")]
        public string Mobile { get; set; }



        [Display(Name = "دسترسی کاربر")]
        [Required(ErrorMessage = "{0} الزامی است.")]
        public long? RoleId { get; set; }



        [Display(Name = "نوع کاربر")]
        [Required(ErrorMessage = "{0} الزامی است.")]
        public UserType Type { get; set; }



        [Display(Name = "فعال است")]
        public bool IsEnabled { get; set; }




        // -----------------------------
        // قرارگاه و یگان
        // -----------------------------


        [Display(Name = "قرارگاه")]
        [Required(ErrorMessage = "{0} الزامی است.")]
        public int? GharargahId { get; set; }



        [Display(Name = "یگان عمده کاربر")]
        [Required(ErrorMessage = "{0} الزامی است.")]
        public int OmdOrgId { get; set; }






        [Display(Name = "استان")]
        public int? ProvinceId { get; set; }



        [Display(Name = "شهر")]
        public int? CityId { get; set; }





        /// <summary>
        /// بروزرسانی نقش ها
        /// </summary>
        public bool ApplyNewAccess { get; set; }




        [Display(Name = "پرسنل مرتبط")]
        public long? PersonId { get; set; }



        [Display(Name = "کد پرسنلی")]
        public int? PersonCode { get; set; }



        [Display(Name = "کد ملی")]
        public int? NationalCode { get; set; }



        [Display(Name = "یگان پادگان")]
        public long? OrganGarrisonId { get; set; }





        public UserEditDTO()
        {
            ApplyNewAccess = false;
        }






        public static Expression<Func<Domain.Entities.User, UserEditDTO>> Selector
        {
            get
            {
                return model => new UserEditDTO()
                {

                    Id = model.Id,

                    Mobile = model.Mobile,

                    Name = model.Name,

                    RoleId = model.RoleId,

                    IsEnabled = model.IsEnabled,

                    Username = model.Username,

                    Type = model.Type,


                    // یگان
                    OmdOrgId = model.OmdOrgId,


                    // قرارگاه
                    GharargahId = model.GharargahId,



                    ProvinceId = model.ProvinceId,

                    CityId = model.CityId,


                    PersonId = model.PersonId,

                    PersonCode = model.PersonCode,

                    NationalCode = model.NationalCode,

                    OrganGarrisonId = model.OrganGarrisonId,

                };
            }
        }


    }
}