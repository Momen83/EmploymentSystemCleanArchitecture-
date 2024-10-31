using EmploymentSystem.Application.Dtos;
using EmploymentSystem.Domain.Interfaces;
using EmploymentSystem.Domain.Models;
using EmploymentSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmploymentSystem.Infrastructure.Repositories
{
    public class VacancyRepository : IVacancyRepository
    {
        protected readonly EmploymentDbContext _dbContext;


        public VacancyRepository(EmploymentDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddApplicationAsync(ApplicationVacancy application)
        {
            await _dbContext.Applications.AddAsync(application);
            await _dbContext.SaveChangesAsync();
        }

        public async Task AddVacancyAsync(Vacancy vacancy)
        {
            await _dbContext.Vacancies.AddAsync(vacancy);
            await _dbContext.SaveChangesAsync();
        }

        public async Task ArchiveVacancyAsync(Vacancy vacancy)
        {
            vacancy.IsArchived = true;
            _dbContext.Vacancies.Update(vacancy);
            await _dbContext.SaveChangesAsync();
        }



        public async Task DeleteVacancyAsync(Vacancy vacancy)
        {
            _dbContext.Vacancies.Remove(vacancy);
            await _dbContext.SaveChangesAsync();

        }

        public async Task<IEnumerable<Vacancy>> GetActiveVacanciesAsync()
        {
            return await _dbContext.Vacancies
                  .Where(vacancy => vacancy.IsActive)
                  .ToListAsync();
        }

        public async Task<IEnumerable<ApplicationVacancy>> GetApplicationsAsync(Vacancy vacancy)
        {

            if (vacancy.Applications == null)
            {

                await _dbContext.Entry(vacancy).Collection(v => v.Applications).LoadAsync();


                foreach (var application in vacancy.Applications)
                {
                    await _dbContext.Entry(application).Reference(a => a.Applicant).LoadAsync();
                }
            }

            return vacancy.Applications;
        }

        public async Task<IEnumerable<ApplicationVacancy>> GetApplicationsAsync()
        {
            return await _dbContext.Applications.ToListAsync();
        }

        public async Task<IEnumerable<Vacancy>> GetExpiredVacanciesAsync()
        {
            return await _dbContext.Vacancies
                               .Where(v => v.ExpiryDate <= DateTime.UtcNow && !v.IsArchived)
                               .ToListAsync();
        }

        public async Task<IEnumerable<Vacancy>> GetVacanciesAsync()
        {
            return await _dbContext.Vacancies.ToListAsync();
        }

        public async Task<Vacancy?> GetVacancyByIdAsync(int id)
        {
            return await _dbContext.Vacancies
            .Include(v => v.Applications)
             .ThenInclude(a => a.Applicant)

            .Include(e => e.Employer)
            .FirstOrDefaultAsync(v => v.Id == id);
        }

        public async Task<IEnumerable<Vacancy>> SearchByTitleAsync(string title, int pageNumber, int pageSize)
        {
            var query = _dbContext.Vacancies
           .Where(v => v.IsActive && !v.IsArchived && v.ExpiryDate >= DateTime.Now && v.Title.Contains(title))
           .OrderByDescending(v => v.ExpiryDate);

           
            return query;

        }

        public async Task UpdateVacancyAsync(Vacancy vacancy)
        {
            var oldVacancy = await _dbContext.Vacancies.FirstOrDefaultAsync(v => v.Id == vacancy.Id);
            if (oldVacancy != null)
            {
                _dbContext.Vacancies.Update(vacancy);
                await _dbContext.SaveChangesAsync();
            }

        }
    }
}
