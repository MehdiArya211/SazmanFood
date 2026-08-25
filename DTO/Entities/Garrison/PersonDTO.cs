using DTO.Base;
using System.Linq.Expressions;

namespace DTO.Entities.Garrison
{
    public class PersonDTO : EntityDTO
    {
        public long? OrganGarrisonId { get; set; }

        public long? PersonTypeId { get; set; }

        public string? FullName { get; set; }

        public string? Name { get; set; }

        public int? PersonCode { get; set; }

        public int? NationalCode { get; set; }

        public string? PersonTypeTitle { get; set; }

        public string? Title { get; set; }

        public string? FoodTitle { get; set; }

        public string? OrganGarrisonOrgName { get; set; }

        public static Expression<Func<Domain.Entities.Garrison.Person, PersonDTO>> Selector
        {
            get
            {
                return model => new PersonDTO()
                {
                    Id = model.Id,

                    OrganGarrisonId = model.OrganGarrisonId,

                    FullName = model.FullName,

                    Name = model.FullName,

                    PersonCode = model.PersonCode,

                    NationalCode = model.NationalCode,

                    PersonTypeId = model.PersonTypeId,

                    OrganGarrisonOrgName = model.OrganGarrison != null
                        ? model.OrganGarrison.Title
                        : "-",

                    PersonTypeTitle = model.PersonalType != null
                        ? model.PersonalType.Title
                        : "-",

                    Title = model.PersonalType != null
                        ? model.PersonalType.Title
                        : "-",

                    FoodTitle = "-"
                };
            }
        }
    }
}