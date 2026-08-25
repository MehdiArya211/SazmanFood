using DTO.DataTable;

namespace BLL.Interface
{
    public interface IDataTableManager
    {
        /// <summary>
        /// گرفتن مدل لازم برای سرچ در دیتاتیبل
        /// </summary>
        DataTableSearchDTO GetSearchModel();

    }
}
