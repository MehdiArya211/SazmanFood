using Domain.Entities;
using DTO.DataTable;
using DTO;
using Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using DTO.Base;
using Services.SessionServices;
using BLL.Interface;
using DTO.Entities;

namespace BLL
{
    public class QoutaAllocationManager : Manager<QoutaAllocation, ApplicationContext>, IQoutaAllocationManager
    {
        protected readonly IHttpContextAccessor httpContextAccessor;
        protected readonly ISession Session;
        protected readonly IFoodPlanManager _foodPlanManager;
        public QoutaAllocationManager(DbContexts _Context, 
            IHttpContextAccessor httpContextAccessor, IFoodPlanManager foodPlanManager) :
            base(_Context, httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor;
            Session = httpContextAccessor.HttpContext.Session;
            _foodPlanManager= foodPlanManager;
        }



        public DataTableResponseDTO<QoutaAllocationDataTableDTO> GetDataTableDTO(
            DataTableSearchDTO searchData,
            QoutaAllocationFilterDataTableDTO filters)
        {
            var user = Session.GetUser();

            var userAccess = new QoutaAllocationUserAccessDTO
            {
                UserId = user.Id,
                RoleTitle = user.Role?.Trim(),
                OrganGarrisonId = user.OrganGarrisonId
            };

            return UOW.QoutaAllocation.GetDataTableDTO(searchData, userAccess, filters);
        }

        public IList<SelectListDTO> GetSelectListDTO()
        {
            throw new NotImplementedException();
        }

        public BaseResult Create(QoutaAllocationCreateDTO model)
        {
            var user = Session.GetUser();
            var dayTitle = _foodPlanManager.GetDayTitle(model.DayId);

            var qoutaAllocation = new QoutaAllocation()
            {
                OrganGarrisonId = model.OrganGarrisonId,
                FoodPlanDayId = model.FoodPlanDayId,
                QoutaAllocationDate = model.QoutaAllocationDate,

                DayId = model.DayId,
                DayTitle = dayTitle,

                MealId = model.MealId,

                OfficerCapacity = model.OfficerCapacity,
                SoldierCapacity = model.SoldierCapacity,
                GuestCapacity = model.GuestCapacity,
                ManagementTokenCapacity = model.ManagementTokenCapacity,
                RegisterDeadline = model.RegisterDeadline,
                AllowPersonChange = model.AllowPersonChange,
                AllowOfficeUserRegister = model.AllowOfficeUserRegister,
                IsFinalized = model.IsFinalized,

                CreateDate = DateTime.Now,
                RegUserId = user.Id,
                IsDeleted = false,
            };

            return base.Create(qoutaAllocation);
        }
        public QoutaAllocationEditDTO GetEditDTO(long? id)
        {
            if (id == null) return null;

            return UOW.QoutaAllocation.GetOneDTO<QoutaAllocationEditDTO>(QoutaAllocationEditDTO.Selector, x => x.Id == id);
        }

        public BaseResult Update(QoutaAllocationEditDTO model)
        {
            var qoutaAllocation = UOW.QoutaAllocation.FirstOrDefault(x => x.Id == model.Id);

            qoutaAllocation.OrganGarrisonId = model.OrganGarrisonId;
            qoutaAllocation.QoutaAllocationDate = model.QoutaAllocationDate;
            qoutaAllocation.DayId = model.DayId;
            qoutaAllocation.DayTitle = model.DayTitle;
            qoutaAllocation.MealId = model.MealId;
            return base.Update(qoutaAllocation);
        }
    }
}
