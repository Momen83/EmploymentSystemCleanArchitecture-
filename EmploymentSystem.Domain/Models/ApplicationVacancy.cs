using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmploymentSystem.Domain.Models
{
    public class ApplicationVacancy
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int VacancyId { get; set; }
        public DateTime ApplicationDate { get; set; }
        public User Applicant { get; set; }
        public Vacancy Vacancy { get; set; }
    }
}
