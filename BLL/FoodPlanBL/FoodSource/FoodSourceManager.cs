using BLL.Interface;
using Domain.Entities;
using DTO.Base;
using DTO.DataTable;
using DTO.Entities;
using Infrastructure.Data;
using Microsoft.AspNetCore.Http;

namespace BLL
{
    public class FoodSourceManager
        : Manager<FoodSource, ApplicationContext>,
          IFoodSourceManager
    {
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly ISession Session;

        public FoodSourceManager(
            DbContexts contexts,
            IHttpContextAccessor httpContextAccessor)
            : base(contexts, httpContextAccessor)
        {
            this.httpContextAccessor =
                httpContextAccessor;

            Session =
                httpContextAccessor.HttpContext?.Session;
        }

        public DataTableResponseDTO<FoodSourceDataTableDTO>
            GetDataTableDTO(
                DataTableSearchDTO searchData,
                FoodSourceFilterDataTableDTO filters)
        {
            return UOW.FoodSource.GetDataTableDTO(
                searchData,
                filters);
        }

        public IList<SelectListDTO>
            GetSelectListPersonalTypeDTO()
        {
            return UOW.PersonalType
                .GetDTO<SelectListDTO>(
                    SelectListDTO.PersonalTypeSelector)
                .ToList();
        }

        public IList<SelectListDTO>
            GetSelectListYeganTypeDTO()
        {
            return UOW.YeganType
                .GetDTO<SelectListDTO>(
                    SelectListDTO.YeganTypeSelector)
                .ToList();
        }

        public BaseResult Create(
            CreatFoodSourceDTO model)
        {
            if (model == null)
            {
                return new BaseResult(
                    false,
                    "اطلاعات ارسالی معتبر نیست.");
            }

            var duplicate =
                UOW.FoodSource.FirstOrDefault(x =>
                    x.YeganTypeId == model.YeganTypeId &&
                    x.PersonalTypeId == model.PersonalTypeId &&
                    x.DayType == model.DayType &&
                    x.IsDeleted != true);

            if (duplicate != null)
            {
                return new BaseResult(
                    false,
                    "برای نوع خدمت، نوع پرسنل و نوع روز انتخاب‌شده قبلاً مأخذ غذایی ثبت شده است.");
            }

            var foodSource = new FoodSource
            {
                YeganTypeId = model.YeganTypeId,
                PersonalTypeId = model.PersonalTypeId,
                DayType = model.DayType,
                PercentBreakfast = model.PercentBreakfast,
                PercentLunch = model.PercentLunch,
                PercentDinner = model.PercentDinner,
                RegUserId = model.UserCreateId,
                CreateDate = DateTime.Now
            };

            return base.Create(foodSource);
        }

        public EditFoodSourceDTO GetEditDTO(long id)
        {
            if (id <= 0)
                return null;

            return UOW.FoodSource
                .GetOneDTO<EditFoodSourceDTO>(
                    EditFoodSourceDTO.Selector,
                    x => x.Id == id);
        }

        public BaseResult Update(
            EditFoodSourceDTO model)
        {
            if (model == null || model.Id <= 0)
            {
                return new BaseResult(
                    false,
                    "مأخذ غذایی مورد نظر یافت نشد.");
            }

            var foodSource =
                UOW.FoodSource.FirstOrDefault(x =>
                    x.Id == model.Id);

            if (foodSource == null)
            {
                return new BaseResult(
                    false,
                    "مأخذ غذایی مورد نظر یافت نشد.");
            }

            var duplicate =
                UOW.FoodSource.FirstOrDefault(x =>
                    x.Id != model.Id &&
                    x.YeganTypeId == model.YeganTypeId &&
                    x.PersonalTypeId == model.PersonalTypeId &&
                    x.DayType == model.DayType &&
                    x.IsDeleted != true);

            if (duplicate != null)
            {
                return new BaseResult(
                    false,
                    "برای نوع خدمت، نوع پرسنل و نوع روز انتخاب‌شده قبلاً مأخذ غذایی ثبت شده است.");
            }

            foodSource.YeganTypeId =
                model.YeganTypeId;

            foodSource.PersonalTypeId =
                model.PersonalTypeId;

            foodSource.DayType =
                model.DayType;

            foodSource.PercentBreakfast =
                model.PercentBreakfast;

            foodSource.PercentLunch =
                model.PercentLunch;

            foodSource.PercentDinner =
                model.PercentDinner;

            foodSource.LastEditUserId =
                model.UserCreateId;

            foodSource.LastEditDate =
                DateTime.Now;

            return base.Update(foodSource);
        }
    }
}