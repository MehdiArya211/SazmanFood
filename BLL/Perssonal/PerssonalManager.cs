using Domain.Entities;
using DTO.Base;
using Infrastructure.Data;
using Microsoft.AspNetCore.Http;

namespace BLL
{
    public class PerssonalManager : Manager<PersonalType, ApplicationContext>, IPersonalManager
    {
        protected readonly IHttpContextAccessor httpContextAccessor;
        protected readonly ISession Session;
        public PerssonalManager(DbContexts _Context, IHttpContextAccessor httpContextAccessor) : base(_Context, httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor;
            Session = httpContextAccessor.HttpContext.Session;
        }

        public IList<SelectListDTO> GetSelectListDTO()
        {
            return UOW.PersonalType.GetDTO<SelectListDTO>(SelectListDTO.PersonalTypeSelector).ToList();
        }
    }
}
