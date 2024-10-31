using EmploymentSystem.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace EmploymentSystem.Domain.Interfaces
{
    public interface IVacancyRepository
    {
        Task<Vacancy?> GetVacancyByIdAsync(int id);
        Task<IEnumerable<Vacancy>> GetActiveVacanciesAsync();
        Task<IEnumerable<Vacancy>> GetVacanciesAsync();
        Task AddApplicationAsync(ApplicationVacancy application);

        Task<IEnumerable<ApplicationVacancy>> GetApplicationsAsync();
        Task<IEnumerable<ApplicationVacancy>> GetApplicationsAsync(Vacancy vacancy);

        Task AddVacancyAsync(Vacancy vacancy);


        Task UpdateVacancyAsync(Vacancy vacancy);
        Task DeleteVacancyAsync(Vacancy vacancy);
        Task ArchiveVacancyAsync(Vacancy vacancy);

        Task<IEnumerable<Vacancy>> GetExpiredVacanciesAsync();

        public Task<IEnumerable<Vacancy>> SearchByTitleAsync(string title, int pageNumber, int pageSize);
    }
}
