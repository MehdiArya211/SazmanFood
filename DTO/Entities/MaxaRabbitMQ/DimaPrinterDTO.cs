using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.Entities.MaxaRabbitMQ
{
    public class Dima_Printer
    {
        public Guid Id { get; set; }
        public Guid CorrelationId { get; set; }
        public string Type { get; set; }
        public int Version { get; set; }
        public string Company_Name { get; set; }
        public long Employee_Id { get; set; }
        public string Full_Name { get; set; }
        public string Meal_Type { get; set; }
        public string Meal_Details { get; set; }
        public DateTime TimeStamp { get; set; }

    }
}
