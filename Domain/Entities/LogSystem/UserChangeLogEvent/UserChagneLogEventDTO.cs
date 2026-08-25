using Domain.Entities;
using Domain.Entities.LogSystem;
using Domain.Enums;
using System.Linq.Expressions;
using Utilities.Extentions;

namespace DTO.Entities.LogSystem.UserChangeLogEvent
{
    public class UserChagneLogEventDTO
    {
        public long Id { get; set; }

        public TrackEventType TrackEventType { get; set; }

        public string TrackEventTypeDescription { get; set; }

        public string TableName { get; set; }

        public long? TableId { get; set; }

        public string NewData { get; set; }

        public string OldData { get; set; }

        public long UserId { get; set; }

        public int? UserOrgId { get; set; }
        public string UserFullName { get; set; }

        public string UserName { get; set; }

        public string IpAddress { get; set; }

        public DateTime LogDateTime { get; set; }

        public string LogDateTimeFa => LogDateTime.ToPersianDateTime().ToString();

        public static Expression<Func<TrackDatabaseLog, UserChagneLogEventDTO>> Selector
        {
            get
            {
                return model => new UserChagneLogEventDTO()
                {
                    Id = model.Id,
                    TrackEventType = model.TrackEventType,
                    TrackEventTypeDescription = model.TrackEventTypeDescription,
                    TableName = model.TableName,
                    IpAddress = model.IpAddress,
                    LogDateTime = model.LogDateTime,
                    NewData = model.NewData,
                    OldData = model.OldData,
                    TableId = model.TableId,
                    //UserUnitId = user.UserUnitId,
                    UserName = model.UserName,
                    UserId = model.UserId,
                    UserFullName = model.UserName,
                };
            }
        }
    }
}
