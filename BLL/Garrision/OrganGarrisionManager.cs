using Domain.Entities.Garrison;
using DTO;
using DTO.Base;
using DTO.DataTable;
using DTO.Entities;
using Infrastructure.Data;
using Microsoft.AspNetCore.Http;

namespace BLL
{
    public class OrganGarrisionManager : Manager<OrganGarrison, ApplicationContext>, IOrganGarrisionManager
    {
        protected readonly IHttpContextAccessor httpContextAccessor;
        protected readonly ISession Session;
        public OrganGarrisionManager(DbContexts _Context, IHttpContextAccessor httpContextAccessor) : base(_Context, httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor;
            Session = httpContextAccessor.HttpContext.Session;
        }

        public DataTableResponseDTO<OrganGarrisonDTO> GetDataTableDTO(DataTableSearchDTO searchData, OrganGarrisonDTO filters)
        {
            var data = UOW.OrganGarrison.GetDataTableDTO(searchData, filters);

            foreach (var item in data.data)
            {
                if (item.ParentId != null)
                {
                    item.ParentTitle = UOW.OrganGarrison.SingleOrDefault(m => m.Id == item.ParentId).Title;

                }
            }
            data.data.OrderByDescending(m => m.ParentTitle != null);
            return data;
        }

        public List<OrganGarrisonType> GetorganGarrisonTypes()
        {
            return UOW.OrganGarrisonType.Get(m => !m.IsDeleted).ToList();
        }


        public IList<SelectListDTO> GetSelectListDTO()
        {
            var res = UOW.OrganGarrison.GetDTO<SelectListDTO>(SelectListDTO.OrganGarrisonSelector, x => x.ParentId == null).ToList();
            return res;
        }

        public IList<SelectListDTO> GetSubSelectListDTO(long parentId)
        {
            return UOW.OrganGarrison
                .GetDTO<SelectListDTO>(SelectListDTO.OrganGarrisonSelector, x => x.ParentId == parentId)
                .ToList();
        }



        public BaseResult Create(OrganGarrisonDTO model, long UserId)
        {

            var orggarii = new OrganGarrison();
            orggarii.OrgId = model.OrgId;
            orggarii.ParentId = model.ParentId;
            orggarii.Title = model.Title;
            orggarii.OrganGarrisonTypeId = model.OrganGarrisonTypeId;
            orggarii.CreateDate = DateTime.Now;
            orggarii.RegUserId = UserId;
            orggarii.EstedadVazife = model.EstedadVazife;
            orggarii.EstedadKadr = model.EstedadKadr;

            var res = base.Create(orggarii);
            return res;


        }


        public List<OrganGarrison> GetParent()
        {
            return UOW.OrganGarrison.GetAll().Where(m => m.ParentId == null).ToList();
        }



        public OrganGarrisonDTO LoadEditForm(long id)
        {
            if (id == null) return null;
            return UOW.OrganGarrison.GetOneDTO<OrganGarrisonDataTableDTO>(OrganGarrisonDataTableDTO.Selector, x => x.Id == id);
        }


        public BaseResult Update(OrganGarrisonDTO model)
        {
            try
            {
                var entity = UOW.OrganGarrison.FirstOrDefault(x => x.Id == model.Id);

                if (entity == null)
                    return new BaseResult(false, "یگان مورد نظر یافت نشد");

                entity.OrgId = model.OrgId;
                entity.ParentId = model.ParentId;
                entity.Title = model.Title;
                entity.OrganGarrisonTypeId = model.OrganGarrisonTypeId;
                entity.EstedadKadr = model.EstedadKadr;
                entity.EstedadVazife = model.EstedadVazife;
                entity.LastEditDate = DateTime.Now;

                return base.Update(entity);
            }
            catch
            {
                return new BaseResult(false, "ویرایش یگان با خطا همراه بوده است");
            }
        }
    }
}
