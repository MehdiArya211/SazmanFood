
using DAL.Garrison;
using DAL.ReserveManagment;
using DAL.UnitStatisticDL;
using Domain.Entities;

namespace DAL.Interface
{

    public interface IUnitOfWork : IDisposable
    {
        #region Auth System
        IMenuRepository Menus { get; }

        IRoleRepository Roles { get; }

        IRoleMenuRepository RoleMenus { get; }

        IUserRepository Users { get; }
        IUserPasswordHistoryRepository PasswordHistorys { get; }

        #endregion

        #region Food

        #region FoodPlan
        IFoodPlanRepository FoodPlan { get; }
        IFoodPlanDayRepository FoodPlanDay { get; }
        IFoodSourceRepository FoodSource { get; }
        IYeganTypeRepository YeganType { get; }
        IYearsRepository Years { get; }
        ISeasonRepository Season { get; }
        IPersonalTypeRepository PersonalType { get; }
        IMealRepository Meal { get; }
        IDaysRepository Days { get; }
        IFoodRepository Food { get; }
        IFoodTypesRepository FoodTypes { get; }
        IKitchensRepository kitchen { get; }
        IDiningHallRepository diningHall { get; }

        #endregion

        #endregion
        #region Unit Calendar

        IUnitCalendarRepository UnitCalendar { get; }

        #endregion
        #region Qouta
        IQoutaAllocationRepository QoutaAllocation { get; }
		IQoutaPersonRepository QoutaPerson { get; }
        #endregion
        #region Unit Qouta
        IUnitQuotaRepository UnitQuota { get; }

        #endregion
        #region UnitQuotaPerson 
        IUnitQuotaPersonRepository UnitQuotaPerson { get; }
        #endregion
        #region Statistic
        IStatisticRepository Statistic { get; }
        IUnitStatisticRepository UnitStatistic { get; }

        IUnitStatisticDetailRepository UnitStatisticDetail { get; }
        #endregion
        #region Guest Extra Food Request

        IGuestExtraFoodRequestRepository
            GuestExtraFoodRequest
        { get; }

        IGuestExtraFoodRequestAttachmentRepository
            GuestExtraFoodRequestAttachment
        { get; }

        #endregion

        #region Extera Food Person
        IGuestExtraFoodRequestPersonRepository
    GuestExtraFoodRequestPerson
        { get; }
        #endregion
        #region Garrison
        IOrganGarrisonRepository OrganGarrison { get; }
		IOrganGarrisonTypeRepository OrganGarrisonType { get; }
		IPersonRepository Person { get; }

		#endregion

		#region LogSystem
		IUserLogRepository UserLogs { get; }
        IUserChangeLogEventRepository UserChangeLogEvent { get; }

        #endregion


        #region Shared
        IConstantRepository Constants { get; }
        #endregion

        #region Serilog
        ISeriLogRepository SeriLogRepository { get; }
        #endregion



        #region FoodReserve  - FoodReserveDetail
        IFoodReserveRepository FoodReserve { get; }
        IFoodReservationDetailRepository FoodReserveDetail { get; }


        #endregion

        bool Commit();

        Task<bool> CommitAsync();

        bool ExecuteNonQuery<TContext>(string Query, params object[] Parameters);

        Task<bool> ExecuteNonQueryAsync<TContext>(string Query, params object[] Parameters);
    }
}
