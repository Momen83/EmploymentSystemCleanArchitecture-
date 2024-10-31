using EmploymentSystem.Application.Dtos;
using EmploymentSystem.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmploymentSystem.Application.Services
{
    public interface IVacancyService
    {
        Task<VacancyDto> CreateVacancy(VacancyDto vacancyDto, string userName);
        Task DeleteVacancy(int vacancyId);

        Task<VacancyDto> UpdateVacancy(VacancyDto vacancy);
        Task<IEnumerable<VacancyDto>> GetVacancies();
        Task ChangeVacancyStatus(int vacancyId);

        Task<IEnumerable<ApplicationDto>> GetVacancyApplicants(int vacancyId);

        Task ApplyForVacancyAsync(int vacancyId, string userName);


        public Task<PagedResult<VacancyDto>> SearchByTitleAsync(string title, int pageNumber, int pageSize);
        Task<IEnumerable<ApplicationVacancy>> GetApplicationsAsync();

        Task ArchiveExpiredVacanciesAsync();
    }
}
