using Domain.Entities;
using Domain.Entities.Garrison;
using DTO.Base;
using DTO.DataTable;
using DTO.Entities.Garrison;
using Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Garrision
{
    public class PersonManager : Manager<Person, ApplicationContext>, IPersonManager
    {
        protected readonly IHttpContextAccessor httpContextAccessor;
        protected readonly ISession Session;
        public PersonManager(DbContexts _Context, IHttpContextAccessor httpContextAccessor) : base(_Context, httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor;
            Session = httpContextAccessor.HttpContext.Session;
        }

        public DataTableResponseDTO<PersonDTO> GetDataTableDTO(DataTableSearchDTO searchData, PersonDTO filters, long? organGarrisonId)
        {
            return UOW.Person.GetDataTableDTO(searchData, filters,organGarrisonId); 
        }




        public BaseResult Create(PersonDTO model) 
        {
            var entity = new Person();
            entity.PersonCode = model.PersonCode;
            entity.NationalCode = model.NationalCode;
            entity.FullName = model.FullName;
            entity.OrganGarrisonId = model.OrganGarrisonId ?? 0;
            entity.PersonTypeId = model.PersonTypeId ?? 0;
            return base.Create(entity);
        }



        public bool ExistPerson(PersonDTO model) 
        {
            if (model.NationalCode != null)
                return UOW.Person.Any(m => m.NationalCode == model.NationalCode && m.IsDeleted == false);
            else
                return UOW.Person.Any(m => m.PersonCode == model.PersonCode && m.IsDeleted == false);

        }
    }
}
