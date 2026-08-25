using DTO.Base;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DTO
{
    public class PersonalInfDTO : EntityDTO
    {
        
        public string personalCode { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MelliCode { get; set; }

        public string FullName { get; set; }

        public string FatherName { get; set; }
        public int RankCode { get; set; }

        [MaxLength(50)]
        public string RankTitle { get; set; }
        public int? BirthPlaceCode { get; set; }
        [MaxLength(100)]
        public string BirthPlaceTitle { get; set; }

        [Column(TypeName = "varchar(10)")]
        public string BirthDate { get; set; }
        public int? IssuePlaceCode { get; set; }

        [MaxLength(100)]
        public string IssuePlaceTitle { get; set; }

        [Column(TypeName = "varchar(10)")]
        public string IssueDate { get; set; }
        public int? GenderCode { get; set; }


        public string SexCodTitle { get; set; }
        public int? BranchCode { get; set; }

        [MaxLength(50)]
        public string BranchTitle { get; set; }
        public int? EmploymentType { get; set; }

        [MaxLength(50)]
        public string EmploymentTitle { get; set; }
        [Column(TypeName = "varchar(10)")]
        public string EmploymentDate { get; set; }

        [Column(TypeName = "varchar(20)")]
        public string Idnumber { get; set; }
        public int? DusCod { get; set; }
        [MaxLength(50)]
        public string StatuseTitle { get; set; }
        public int UnitCode { get; set; }
        [MaxLength(200)]
        public string UnitTitle { get; set; }
        public int UnitDutyCode { get; set; }
        [MaxLength(200)]
        public string UnitDutyTitle { get; set; }

        //
        [Display(Name = "کد قرارگاه منطقه ای / ارشد نظامی ")]
        [MaxLength(200, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد .")]
        public string CodGhaTitle { get; set; }
        [Display(Name = "نام قرارگاه منطقه ای / ارشد نظامی ")]

        public int? CodGha { get; set; }


        [Display(Name = "کد پشتیبانی منطقه ای  ")]
        [MaxLength(200, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد .")]
        public string PoshOrgTitle { get; set; }
        [Display(Name = "نام پشتیبانی منطقه ای ")]

        public int? PoshOrgCd { get; set; }
    }
}
