using Domain.Entities.LogSystem;
using System.Linq.Expressions;

namespace DTO
{
    public  class SeriLogDataTableDTO
    {
        public long Id { get; set; }
        public string Message { get; set; }
        public string MessageTemplate { get; set; }
        public string Level { get; set; }
        public DateTime TimeStamp { get; set; }
        public string Exception { get; set; }
        public string Properties { get; set; }
        public string LogEvent { get; set; }

        public static Expression<Func<SeriLog, SeriLogDataTableDTO>> Selector
        {
            get
            {
                return model => new SeriLogDataTableDTO()
                {
                    Id = model.Id,
                    Message = model.Message,
                    MessageTemplate = model.MessageTemplate,
                    Level = model.Level,
                    TimeStamp = model.TimeStamp,
                    Exception = model.Exception,
                    Properties = model.Properties,
                    LogEvent = model.LogEvent,

                };
            }
        }
    }
}
