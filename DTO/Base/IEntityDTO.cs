using System;
using System.Collections.Generic;
using System.Text;

namespace DTO.Base
{
    public interface IEntityDTO<TPrimaryKey>
    {
        TPrimaryKey Id { get; set; }
    }
}
