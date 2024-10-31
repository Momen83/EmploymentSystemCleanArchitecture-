using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace EmploymentSystem.Domain.Models
{
    public class Vacancy
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int MaxApplications { get; set; }

        public int EmployerId { get; set; }

        public DateTime ExpiryDate { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsArchived { get; set; } = false;

        public User Employer { get; set; }
        public List<ApplicationVacancy> Applications { get; set; }
    }
}
