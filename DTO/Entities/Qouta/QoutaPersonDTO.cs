using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;

namespace DTO;

public class QoutaPersonDTO
{
    public long Id { get; set; }

    [Display(Name = "نام ")]
    [Required(ErrorMessage = "{0} الزامی است.")]
    [MaxLength(200, ErrorMessage = "{0} حداکثر می تواند {1} کاراکتر باشد.")]
    public string FName { get; set; }

    [Display(Name = "نام خانوادگی ")]
    [Required(ErrorMessage = "{0} الزامی است.")]
    [MaxLength(200, ErrorMessage = "{0} حداکثر می تواند {1} کاراکتر باشد.")]
    public string LName { get; set; }

    [Display(Name = "کد پرسنلی ")]
    [Required(ErrorMessage = "{0} الزامی است.")]
    [MaxLength(200, ErrorMessage = "{0} حداکثر می تواند {1} کاراکتر باشد.")]
    public string PersonalCode { get; set; }

    [Display(Name = "شناسه سهمیه بندی ")]
    [Required(ErrorMessage = "{0} الزامی است.")]
    public long QoutaAllocationId { get; set; }

    [Display(Name = "شناسه یگان ")]
    public long? OrgId { get; set; }
    [Display(Name = "نام ")]
    [MaxLength(200, ErrorMessage = "{0} حداکثر می تواند {1} کاراکتر باشد.")]
    public string OrgTitle { get; set; }
    [Display(Name = "نوع پرسنل ")]
    public long PersonalTypeId { get; set; }

    public int EstedadeKadr { get; set; }
    public int EstedadeVazife { get; set; }

    [Display(Name = "وعده ی غذایی")]
    public string MealTitle { get; set; }
    public string FoodTitle { get; set; }
    public List<FoodPlanDayListForSahmiyebandiDTO> Foods { get; set; }=new List<FoodPlanDayListForSahmiyebandiDTO> { };
    public List<QoutaPersonDataTableDTO> Personels { get; set; }=new List<QoutaPersonDataTableDTO> { };
}

public class QoutaPersonDataTableDTO : QoutaPersonDTO
{
    public long? PersonId { get; set; }

    public static Expression<Func<Domain.Entities.QoutaPerson, QoutaPersonDataTableDTO>> Selector
    {
        get
        {
            return model => new QoutaPersonDataTableDTO()
            {
                Id = model.Id,
                PersonId = model.PersonId,

                FName = model.FName,
                LName = model.LName,
                PersonalCode = model.PersonalCode,

                QoutaAllocationId = model.QoutaAllocationId,
                OrgId = model.OrgId,
                OrgTitle = model.OrgTitle,

                FoodTitle = model.MainFood != null ? model.MainFood.Title : "-",

                PersonalTypeId = model.PersonalTypeId.HasValue
                    ? model.PersonalTypeId.Value
                    : 0
            };
        }
    }
}

public class QoutaPersonCreateDTO : QoutaPersonDTO
{
    [Display(Name = "غذا")]
    public long? FoodId { get; set; }

    [Display(Name = "شناسه پرسنل")]
    public long PersonId { get; set; } // ✅ اضافه شد
}

public class OrganSahmiyeDTO
{
    public int EstedadeKadr { get; set; }
    public int EstedadeVazife { get; set; }
    public string MealTitle { get; set; }
}

public class QoutaPersonEditDTO : QoutaPersonDTO
{
    public static Expression<Func<Domain.Entities.QoutaPerson, QoutaPersonDataTableDTO>> Selector
    {
        get
        {
            return model => new QoutaPersonDataTableDTO()
            {
                Id = model.Id,
                FName = model.FName,
                LName = model.LName,
                PersonalCode = model.PersonalCode,
                QoutaAllocationId = model.QoutaAllocationId,
                OrgId = model.OrgId ?? 0,
                OrgTitle = model.OrgTitle,
                PersonalTypeId = model.PersonalTypeId.Value,


            };
        }
    }
}
