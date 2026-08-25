using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BLL.Interface;
using DAL;
using DAL.Interface;
using DTO.Base;
using DTO.DataTable;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;



namespace BLL
{
    public class DataTableManager : IDataTableManager
    {
        IHttpContextAccessor httpContextAccessor;
        public DataTableManager(IHttpContextAccessor _httpContextAccessor) 
        {
            httpContextAccessor = _httpContextAccessor;
        }



        /// <summary>
        /// گرفتن مدل لازم برای سرچ
        /// </summary>
        /// <returns></returns>
        public DataTableSearchDTO GetSearchModel()
        {
            var Request = httpContextAccessor.HttpContext.Request;
            var model = new DataTableSearchDTO
            {
                start = Convert.ToInt32(Request.Form["start"]),
                length = Convert.ToInt32(Request.Form["length"]),
                searchValue = Request.Form["search[value]"],
                sortColumnName = Request.Form["columns[" + Request.Form["order[0][column]"] + "][data]"],
                sortDirection = Request.Form["order[0][dir]"],
                draw = Request.Form["draw"]
            };
            return model;
        }


    }
}
