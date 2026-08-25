using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Spreadsheet;
using Domain.Entities;
using DTO.DataTable;
using DTO.Entities.LogSystem.UserChangeLogEvent;
using Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using StackExchange.Redis;
using System.Data;

namespace BLL.LogSystem.UserChangeLogEvent;

public class UserChangeLogEventManager : Manager<DbChangeLogEvent, LogContext>, IUserChangeLogEventManager
{
    public UserChangeLogEventManager(DbContexts contexts, IHttpContextAccessor httpContextAccessor) : base(contexts, httpContextAccessor)
    {
    }

    public DataTableResponseDTO<UserChagneLogEventDataTableDTO> GetDataTableDTO(DataTableSearchDTO searchData, UserChagneLogEventFilter filters)
    {
        //#region گرفتن تمام فیلد های یک جدول

        //List<string> listColumns = new List<string>();
        //List<string> listTables = new List<string>();
        //using (SqlConnection conection=new SqlConnection("Server=10.128.155.42;Database=LearningSkill;User Id=learnskill;password=!QAZ2wsx;TrustServerCertificate=True;MultipleActiveResultSets=true"))
        //    using (SqlCommand command= conection.CreateCommand())
        //{
        //    command.CommandText = "select c.name from sys.columns c inner join sys.tables t on t.object_id = c.object_id and t.name = 'UserChangeLogEvents' and t.type= 'U' ";
        //    conection.Open();
        //   DataTable allTables= conection.GetSchema("Tables");
        //    foreach (DataRow row in allTables.Rows)
        //    {
        //        string tableName = (string)row[2];
        //        listTables.Add(tableName);
        //    }

        //    var resTables = listTables;

        //    using (var reader = command.ExecuteReader())
        //    {
        //        while (reader.Read())
        //        {
        //            listColumns.Add(reader.GetString(0));
        //        }
        //    }

        //    var res=listColumns.ToArray();
        //}
        //    #endregion
            return UOW.UserChangeLogEvent.GetDataTableDTO(searchData, filters);
    }

    public UserChagneLogEventDTO GetDetailDTO(long id)
    {
        return UOW.UserChangeLogEvent.GetDetailDTO(id);
    }
}
