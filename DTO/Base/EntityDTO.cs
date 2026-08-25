using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DTO.Base
{
    public class EntityDTO : IEntityDTO<long>
    {
        [Display(Name = "شناسه")]
        public long Id { get; set; }
    }
}
