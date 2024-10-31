using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmploymentSystem.Application.Dtos
{
    public class ApplicationDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string VacacnyTitle { get; set; }
        public int VacancyId { get; set; }
        public DateTime ApplicationDate { get; set; }
    }
}
