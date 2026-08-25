using DAL.Food.KitchenDL;
using DAL.Garrison;
using DAL.Interface;
using DAL.LogSystem.UserChangeLogEvent;
using DAL.ReserveManagment;
using DAL.UnitStatisticDL;
using Domain.Entities;
using Domain.Entities.FoodReservation;
using Domain.Entities.LogSystem;
using Domain.Enums;
using Infrastructure.Data;
using Infrastructure.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Services.SessionServices;
using Utilities.Extentions;

namespace DAL
{


    public class UnitOfWork : IUnitOfWork
    {
        protected readonly ApplicationContext applicationContext;
        protected readonly LogContext logContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ISession _session;

        public UnitOfWork(DbContexts contexts, IHttpContextAccessor httpContextAccessor)
        {
            applicationContext = contexts[DbContextType.ApplicationContext.ToString()] as ApplicationContext;
            logContext = contexts[DbContextType.LogContext.ToString()] as LogContext;
            _httpContextAccessor = httpContextAccessor;
            _session = httpContextAccessor.HttpContext?.Session;

        }




        #region private properties

        #region Auth System
        private MenuRepository _menus;
        private RoleRepository _roles;
        private RoleMenuRepository _roleMenus;
        private UserRepository _users;
        private UserPasswordHistoryRepository _PasswordHistory;
        #endregion

        #region LogSystem
        private UserLogRepository _userLogs;
        private UserChangeLogEventRepository _userChangeLogEvent;
        #endregion

        #region Shared
        private ConstantRepository _constants;
        #endregion

        #region SeriLog
        private SeriLogRepository _seriLogs;
        #endregion

        #region Food

        #region FoodPlan
        private FoodPlanRepository _FoodPlan;
        private FoodPlanDayRepository _FoodPlanDay;
        private FoodSourceRepository _FoodSource;
        private YeganTypeRepository _YeganType;
        private YearsRepository _Years;
        private SeasonRepository _Season;
        private PersonalTypeRepository _PersonalType;
        private MealRepository _Meal;
        private DaysRepository _Days;
        private FoodRepository _Food;
        private FoodTypesRepository _FoodTypes;
        private KitchensRepository _kitchens;
        private DiningHallRepository _diningHall;

        #endregion

        #endregion

        #region Qouta
        private QoutaAllocationRepository _QoutaAllocation;
		private QoutaPersonRepository _QoutaPerson;

        #endregion
        #region Unit Calendar

        private UnitCalendarRepository _UnitCalendar;

        #endregion

        #region Extera Food Person
        private IGuestExtraFoodRequestPersonRepository
    guestExtraFoodRequestPerson;
        #endregion

        #region Extera Food
        private GuestExtraFoodRequestRepository
    _GuestExtraFoodRequest;

        private GuestExtraFoodRequestAttachmentRepository
            _GuestExtraFoodRequestAttachment;
        #endregion
        #region Unit Quota
        private UnitQuotaRepository _UnitQuota;
        #endregion
        #region Unit Quota person
        private UnitQuotaPersonRepository _UnitQuotaPerson;
        #endregion

        #region Statistic
        private StatisticRepository _Statistic;
        private UnitStatisticRepository _UnitStatistic;
        private UnitStatisticDetailRepository _UnitStatisticDetail;
        #endregion

        #region Garrison
        private OrganGarrisonRepository _OrganGarrison;
		private OrganGarrisonTypeRepository _OrganGarrisonType;
		private PersonRepository _PersonRepository;


        #endregion


        #region FoodReserve FoodReserveDetail
        private FoodReserveRepository _FoodReserve;
        private FoodReservationDetailRepository _FoodReserveDetail;

        #endregion

        #endregion

        #region Repository Getters 

        #region Auth System
        public IMenuRepository Menus
        {
            get
            {
                if (_menus == null)
                    _menus = new MenuRepository(applicationContext);
                return _menus;
            }
        }

        public IRoleRepository Roles
        {
            get
            {
                if (_roles == null)
                    _roles = new RoleRepository(applicationContext);
                return _roles;
            }
        }

        public IRoleMenuRepository RoleMenus
        {
            get
            {
                if (_roleMenus == null)
                    _roleMenus = new RoleMenuRepository(applicationContext);
                return _roleMenus;
            }
        }

        public IUserRepository Users
        {
            get
            {
                if (_users == null)
                    _users = new UserRepository(applicationContext);
                return _users;
            }
        }
        public IUserPasswordHistoryRepository PasswordHistorys
        {
            get
            {
                if (_PasswordHistory == null)
                    _PasswordHistory = new UserPasswordHistoryRepository(applicationContext);
                return _PasswordHistory;
            }
        }

        #endregion

        #region Food

        #region FoodPlan

        public IFoodPlanRepository FoodPlan
        {
            get
            {
                if (_FoodPlan == null)
                    _FoodPlan = new FoodPlanRepository(applicationContext);
                return _FoodPlan;
            }
        }

        public IFoodPlanDayRepository FoodPlanDay
        {
            get
            {
                if (_FoodPlanDay == null)
                    _FoodPlanDay = new FoodPlanDayRepository(applicationContext);
                return _FoodPlanDay;
            }
        }
        public IFoodSourceRepository FoodSource
        {
            get
            {
                if (_FoodSource == null)
                    _FoodSource = new FoodSourceRepository(applicationContext);
                return _FoodSource;
            }
        }
        public IYeganTypeRepository YeganType
        {
            get
            {
                if (_YeganType == null)
                    _YeganType = new YeganTypeRepository(applicationContext);
                return _YeganType;
            }
        }
        public IYearsRepository Years
        {
            get
            {
                if (_Years == null)
                    _Years = new YearsRepository(applicationContext);
                return _Years;
            }
        }
        public ISeasonRepository Season
        {
            get
            {
                if (_Season == null)
                    _Season = new SeasonRepository(applicationContext);
                return _Season;
            }
        }
        public IPersonalTypeRepository PersonalType
        {
            get
            {
                if (_PersonalType == null)
                    _PersonalType = new PersonalTypeRepository(applicationContext);
                return _PersonalType;
            }
        }
        public IMealRepository Meal
        {
            get
            {
                if (_Meal == null)
                    _Meal = new MealRepository(applicationContext);
                return _Meal;
            }
        }
        public IDaysRepository Days
        {
            get
            {
                if (_Days == null)
                    _Days = new DaysRepository(applicationContext);
                return _Days;
            }
        }
        public IFoodRepository Food
        {
            get
            {
                if (_Food == null)
                    _Food = new FoodRepository(applicationContext);
                return _Food;
            }
        }
        public IFoodTypesRepository FoodTypes
        {
            get
            {
                if (_FoodTypes == null)
                    _FoodTypes = new FoodTypesRepository(applicationContext);
                return _FoodTypes;
            }
        }
        public IKitchensRepository kitchen
        {
            get
            {
                if (_kitchens == null)
                    _kitchens = new KitchensRepository(applicationContext);
                return _kitchens;
            }
        }
        public IDiningHallRepository diningHall
        {
            get
            {
                if (_diningHall == null)
                    _diningHall = new DiningHallRepository(applicationContext);
                return _diningHall;
            }
        }

        #endregion

        #endregion

        #region Qouta
        public IQoutaAllocationRepository QoutaAllocation
		{
			get
			{
				if (_QoutaAllocation == null)
					_QoutaAllocation = new QoutaAllocationRepository(applicationContext);
				return _QoutaAllocation;
			}
		}
		public IQoutaPersonRepository QoutaPerson
		{
			get
			{
				if (_QoutaPerson == null)
					_QoutaPerson = new QoutaPersonRepository(applicationContext);
				return _QoutaPerson;
			}
		}
        #endregion

        #region UnitQuotaperson
        public IUnitQuotaPersonRepository UnitQuotaPerson
        {
            get
            {
                if (_UnitQuotaPerson == null)
                {
                    _UnitQuotaPerson =
                        new UnitQuotaPersonRepository(
                            applicationContext);
                }

                return _UnitQuotaPerson;
            }
        }
        #endregion
        #region Unit Calendar

        public IUnitCalendarRepository UnitCalendar
        {
            get
            {
                if (_UnitCalendar == null)
                {
                    _UnitCalendar =
                        new UnitCalendarRepository(
                            applicationContext);
                }

                return _UnitCalendar;
            }
        }

        #endregion

        #region ExteraFood
        public IGuestExtraFoodRequestRepository
    GuestExtraFoodRequest
        {
            get
            {
                if (_GuestExtraFoodRequest == null)
                {
                    _GuestExtraFoodRequest =
                        new GuestExtraFoodRequestRepository(
                            applicationContext);
                }

                return _GuestExtraFoodRequest;
            }
        }

        public IGuestExtraFoodRequestAttachmentRepository
            GuestExtraFoodRequestAttachment
        {
            get
            {
                if (_GuestExtraFoodRequestAttachment == null)
                {
                    _GuestExtraFoodRequestAttachment =
                        new GuestExtraFoodRequestAttachmentRepository(
                            applicationContext);
                }

                return _GuestExtraFoodRequestAttachment;
            }
        }
        #endregion

        #region Extera Food Person
        public IGuestExtraFoodRequestPersonRepository
    GuestExtraFoodRequestPerson
        {
            get
            {
                return guestExtraFoodRequestPerson ??=
                    new GuestExtraFoodRequestPersonRepository(applicationContext);
            }
        }
        #endregion
        #region Unit Qouta
        public IUnitQuotaRepository UnitQuota
        {
            get
            {
                if (_UnitQuota == null)
                {
                    _UnitQuota =
                        new UnitQuotaRepository(
                            applicationContext);
                }

                return _UnitQuota;
            }
        }
        #endregion

        #region Gerrison

        public IPersonRepository Person
		{
			get
			{
				if (_PersonRepository == null)
					_PersonRepository = new PersonRepository(applicationContext);
				return _PersonRepository;
			}
		}
		public IOrganGarrisonRepository OrganGarrison
		{
			get
			{
				if (_OrganGarrison == null)
					_OrganGarrison = new OrganGarrisonRepository(applicationContext);
				return _OrganGarrison;
			}
		}
		public IOrganGarrisonTypeRepository OrganGarrisonType
		{
			get
			{
				if (_OrganGarrisonType == null)
					_OrganGarrisonType = new OrganGarrisonTypeRepository(applicationContext);
				return _OrganGarrisonType;
			}
		}
        #endregion

        #region Statistic

        public IUnitStatisticRepository UnitStatistic
        {
            get
            {
                if (_UnitStatistic == null)
                {
                    _UnitStatistic =
                        new UnitStatisticRepository(
                            applicationContext);
                }

                return _UnitStatistic;
            }
        }

        public IUnitStatisticDetailRepository UnitStatisticDetail
        {
            get
            {
                if (_UnitStatisticDetail == null)
                {
                    _UnitStatisticDetail =
                        new UnitStatisticDetailRepository(
                            applicationContext);
                }

                return _UnitStatisticDetail;
            }
        }

        public IStatisticRepository Statistic
		{
			get
			{
				if (_Statistic == null)
					_Statistic = new StatisticRepository(applicationContext);
				return _Statistic;
			}
		}
		#endregion


		#region LogSystem
		public IUserLogRepository UserLogs
        {
            get
            {
                if (_userLogs == null)
                    _userLogs = new UserLogRepository(logContext);
                return _userLogs;
            }
        }

        public IUserChangeLogEventRepository UserChangeLogEvent
        {
            get
            {
                if (_userChangeLogEvent == null)
                    _userChangeLogEvent = new UserChangeLogEventRepository(logContext);
                return _userChangeLogEvent;
            }
        }

        #endregion


        #region Shared
        public IConstantRepository Constants
        {
            get
            {
                if (_constants == null)
                    _constants = new ConstantRepository(applicationContext);
                return _constants;
            }
        }

        #endregion


        #region SeriLog
        public ISeriLogRepository SeriLogRepository
        {
            get
            {
                if (_seriLogs == null)
                    _seriLogs = new SeriLogRepository(applicationContext);
                return _seriLogs;
            }
        }


        public IFoodReserveRepository FoodReserve
        {
            get
            {
                if (_FoodReserve == null)
                    _FoodReserve = new FoodReserveRepository(applicationContext);
                return _FoodReserve;
            }
        }


        public IFoodReservationDetailRepository FoodReserveDetail
        {
            get
            {
                if (_FoodReserveDetail == null)
                    _FoodReserveDetail = new FoodReservationDetailRepository(applicationContext);
                return _FoodReserveDetail;
            }
        }


        #endregion
        #endregion


        #region Commit
        /// <summary>
        /// ذخیره تغییرات انجام شده
        /// </summary>
        /// <returns></returns>
        public bool Commit()
        {
            try
            {
                if (applicationContext.ChangeTracker.HasChanges())
                {
                    #region DbChangeLogEvent | ثبت تغییرات پایگاه داده

                    var changeLog = new List<TrackDatabaseLog>();

                    try
                    {
                        foreach (var item in applicationContext.ChangeTracker.Entries())
                        {
                            var originalValues = "";
                            if (item.State == EntityState.Modified)
                            {
                                var type = item.Entity.GetType();
                                var entity = Activator.CreateInstance(type);

                                foreach (var prop in item.OriginalValues.Properties)
                                {
                                    try
                                    {
                                        prop.PropertyInfo?.SetValue(entity, item.OriginalValues[prop]);
                                    }
                                    catch (Exception)
                                    {
                                    }
                                }

                                originalValues = entity.ToJson();
                            }

                            var user = _session?.GetUser();
                            var state = item.State;
                            var newData = item.Entity.ToJson();
                            var entityType = item.OriginalValues.EntityType;
                            var entityId = item.Properties.FirstOrDefault(wh => wh.Metadata.Name == "Id")?.OriginalValue ?? 0;

                            changeLog.Add(new TrackDatabaseLog
                            {
                                UserId = user?.Id ?? 0,
                                UserName = user?.Username,
                                //UserUnitId = user.UserUnitId,
                                UserFullName = user?.FullName ?? "",
                                NewData = newData,
                                TableId = (long)entityId,
                                OldData = originalValues,
                                LogDateTime = DateTime.Now,
                                TrackEventType = (TrackEventType)state,
                                TrackEventTypeDescription = state.ToString(),
                                TableName = entityType.ConstructorBinding?.RuntimeType.Name,
                                IpAddress = _httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString(),
                            });
                        }
                    }
                    catch (Exception)
                    {

                    }

                    #endregion

                    applicationContext.SaveChanges();

                    #region DbChangeLogEvent | به روز رسانی شناسه به هنگام ایجاد تغییرات پایگاه داده

                    if (changeLog.Any())
                    {
                        try
                        {
                            foreach (var item in applicationContext.ChangeTracker.Entries())
                            {
                                var newData = item.Entity.ToJson();
                                var entityType = item.OriginalValues.EntityType;
                                var entityId = item.Properties.FirstOrDefault(wh => wh.Metadata.Name == "Id")?.OriginalValue ?? 0;

                                var oldEvent = changeLog.FirstOrDefault(wh => wh.TrackEventType == TrackEventType.Added &&
                                                                              wh.TableName == entityType.ConstructorBinding?.RuntimeType.Name);
                                if (oldEvent == null) continue;
                                oldEvent.TableId = (long)entityId;
                                oldEvent.NewData = newData;
                            }
                        }
                        catch (Exception)
                        {

                        }

                        logContext.UserChangeLogEvents.AddRange(changeLog);
                    }

                    #endregion
                }

                if (logContext.ChangeTracker.HasChanges())
                    logContext.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }



        /// <summary>
        /// ذخیره تغییرات انجام شده
        /// </summary>
        /// <returns></returns>
        public async Task<bool> CommitAsync()
        {
            try
            {
                if (applicationContext.ChangeTracker.HasChanges())
                {
                    #region DbChangeLogEvent | ثبت تغییرات پایگاه داده

                    var changeLog = new List<TrackDatabaseLog>();

                    try
                    {
                        foreach (var item in applicationContext.ChangeTracker.Entries())
                        {
                            var originalValues = "";
                            if (item.State == EntityState.Modified)
                            {
                                Type type = item.Entity.GetType();
                                var entity = Activator.CreateInstance(type);

                                foreach (var prop in item.OriginalValues.Properties)
                                {
                                    prop.PropertyInfo.SetValue(entity, item.OriginalValues[prop]);
                                }

                                originalValues = entity.ToJson();
                            }

                            var user = _session?.GetUser();
                            var state = item.State;
                            var newData = item.Entity.ToJson();
                            var entityType = item.OriginalValues.EntityType;
                            var entityId = item.Properties.FirstOrDefault(wh => wh.Metadata.Name == "Id")?.OriginalValue ?? 0;

                            changeLog.Add(new TrackDatabaseLog
                            {
                                UserId = user?.Id ?? 0,
                                UserName = user?.Username ?? "",
                                //UserUnitId = user.UserUnitId,
                                UserFullName = user.FullName,
                                NewData = newData,
                                TableId = (long)entityId,
                                OldData = originalValues,
                                LogDateTime = DateTime.Now,
                                TrackEventType = (TrackEventType)state,
                                TrackEventTypeDescription = state.ToString(),
                                TableName = entityType.ConstructorBinding.RuntimeType.Name,
                                IpAddress = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString(),
                            });
                        }
                    }
                    catch (Exception)
                    {

                    }

                    #endregion

                    await applicationContext.SaveChangesAsync();

                    #region DbChangeLogEvent | به روز رسانی شناسه به هنگام ایجاد تغییرات پایگاه داده

                    if (changeLog.Any())
                    {
                        try
                        {
                            foreach (var item in applicationContext.ChangeTracker.Entries())
                            {
                                var newData = item.Entity.ToJson();
                                var entityType = item.OriginalValues.EntityType;
                                var entityId = item.Properties.FirstOrDefault(wh => wh.Metadata.Name == "Id")?.OriginalValue ?? 0;

                                var oldEvent = changeLog.Where(wh =>
                                    wh.TrackEventType == TrackEventType.Added &&
                                    wh.TableName == entityType.ConstructorBinding.RuntimeType.Name).FirstOrDefault();
                                if (oldEvent != null)
                                {
                                    oldEvent.TableId = (long)entityId;
                                    oldEvent.NewData = newData;
                                }
                            }
                        }
                        catch (Exception)
                        {

                        }

                        logContext.UserChangeLogEvents.AddRange(changeLog);
                    }

                    #endregion
                }

                if (logContext.ChangeTracker.HasChanges())
                    await logContext.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        #endregion


        #region ExecuteNonQuery

        /// <summary>
        /// اجرای یک کوئری بدون مقدار بازگشتی
        /// </summary>
        /// <param name="Query">متن کوئری</param>
        /// <param name="Parameters">پارامتر های کوئری</param>
        /// <returns></returns>
        public bool ExecuteNonQuery<TContext>(string Query, params object[] Parameters)
        {
            try
            {
                if (typeof(TContext) == typeof(LogContext))
                {
                    int NumberOfRowEffected = logContext.Database.ExecuteSqlRaw(Query, Parameters);
                    return true;
                }
                else
                {
                    int NumberOfRowEffected = applicationContext.Database.ExecuteSqlRaw(Query, Parameters);
                    return true;
                }
            }
            catch (Exception e)
            {
                return false;
            }
        }



        /// <summary>
        /// اجرای یک کوئری بدون مقدار بازگشتی
        /// </summary>
        /// <param name="Query">متن کوئری</param>
        /// <param name="Parameters">پارامتر های کوئری</param>
        /// <returns></returns>
        public async Task<bool> ExecuteNonQueryAsync<TContext>(string Query, params object[] Parameters)
        {
            try
            {
                if (typeof(TContext) == typeof(LogContext))
                {
                    int NumberOfRowEffected = await logContext.Database.ExecuteSqlRawAsync(Query, Parameters);
                    return true;
                }
                else
                {
                    int NumberOfRowEffected = await applicationContext.Database.ExecuteSqlRawAsync(Query, Parameters);
                    return true;
                }
            }
            catch (Exception e)
            {
                return false;
            }
        }

        #endregion


        /// <summary>
        /// پاک کردن آبجکت unit of work و Context از رم
        /// </summary>
        public void Dispose()
        {
            applicationContext.Dispose();
            logContext.Dispose();
        }


    }
}
