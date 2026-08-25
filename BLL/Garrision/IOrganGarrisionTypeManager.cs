using BLL.Interface;
using Domain.Entities.Garrison;
using Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public interface IOrganGarrisionTypeManager : IManager<OrganGarrisonType, ApplicationContext>
	{
	}
}
