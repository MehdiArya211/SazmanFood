using BLL.Interface;
using DocumentFormat.OpenXml.Spreadsheet;
using Domain.Entities;
using Infrastructure.Data;

namespace BLL;

public interface IYearsManager : IManager<Years, ApplicationContext>
{
    public long? GetActiveYearId();


}
