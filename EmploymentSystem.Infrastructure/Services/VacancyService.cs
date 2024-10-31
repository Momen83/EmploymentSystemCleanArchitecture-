using AutoMapper;
using EmploymentSystem.Application.Dtos;
using EmploymentSystem.Application.Services;
using EmploymentSystem.Domain.Interfaces;
using EmploymentSystem.Domain.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace EmploymentSystem.Infrastructure.Services
{
    public class VacancyService : IVacancyService
    {
        private readonly IVacancyRepository _vacancyRepository;
        private readonly IMemoryCache _cache;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<VacancyService> _logger;
        private const string VacancyCacheKey = "AllVacancies";

        public VacancyService(IVacancyRepository vacancyRepository, IMapper mapper, IUserRepository userRepository
             , IMemoryCache cache , ILogger<VacancyService> logger
            )
        {
            _vacancyRepository = vacancyRepository;
            _mapper = mapper;
            _userRepository = userRepository;
            _cache = cache;
            _logger = logger;

        }
        public async Task ChangeVacancyStatus(int vacancyId)
        {
            var vacancy = await _vacancyRepository.GetVacancyByIdAsync(vacancyId);
            if (vacancy == null)
            {
                _logger.LogError($"Vacacny with id {vacancyId} not found to change status");
                throw new Exception("Vacacny not found");
            }

            vacancy.IsActive = !vacancy.IsActive;
            await _vacancyRepository.UpdateVacancyAsync(vacancy);
            _logger.LogInformation($"Vacacny with id {vacancyId} changed status");
            _cache.Remove(VacancyCacheKey);

        }


        public async Task<VacancyDto> CreateVacancy(VacancyDto vacancyDto, string userName)
        {

            if (vacancyDto.MaxApplications < 0)
            {
                _logger.LogError("Maximum number of application  not valid");
                throw new Exception("Maximum number of application  not valid");

            }

            vacancyDto.IsActive = true;
            var vacancy = _mapper.Map<Vacancy>(vacancyDto);
            var employer = await _userRepository.GetUserByUsernameAsync(userName);

            vacancy.EmployerId = employer.Id;
            vacancy.Employer = employer;

            await _vacancyRepository.AddVacancyAsync(vacancy);
            vacancyDto.Id = vacancy.Id;

            _logger.LogInformation($"Vacacny with id {vacancyDto.Id} is created successfully");
            _cache.Remove(VacancyCacheKey); // Invalidate cache after add operation
            return vacancyDto;
        }

        public async Task DeleteVacancy(int vacancyId)
        {
            var vacancy = await _vacancyRepository.GetVacancyByIdAsync(vacancyId);
            if (vacancy == null)
            {
                _logger.LogError($"Vacacny with id {vacancyId} not found to delete");
                throw new Exception("Vacacny not found");
            }

            await _vacancyRepository.DeleteVacancyAsync(vacancy);
            _logger.LogInformation($"Vacacny with id {vacancyId} deleted");
            _cache.Remove(VacancyCacheKey);
        }

        public async Task<IEnumerable<VacancyDto>> GetVacancies()
        {
            if (!_cache.TryGetValue(VacancyCacheKey, out IEnumerable<Vacancy> vacancies))
            {
                _logger.LogInformation("Cache miss for vacancies. Fetching from database...");
                vacancies = await _vacancyRepository.GetVacanciesAsync();

                var cacheEntryOptions = new MemoryCacheEntryOptions()
              .SetSlidingExpiration(TimeSpan.FromMinutes(30)) // Sliding expiration
              .SetAbsoluteExpiration(TimeSpan.FromHours(1));
                _cache.Set(VacancyCacheKey, vacancies, cacheEntryOptions);

                _logger.LogInformation("Vacancies cached successfully.");
            }

            else
            {
                _logger.LogInformation("Cache hit for vacancies. Returning data from cache.");
            }


            var vacancyDtos = _mapper.Map<List<VacancyDto>>(vacancies);
            return vacancyDtos;

        }

        public async Task<IEnumerable<ApplicationDto>> GetVacancyApplicants(int vacancyId)
        {
            var vacancy = await _vacancyRepository.GetVacancyByIdAsync(vacancyId);
            if (vacancy == null)
            {
                _logger.LogError($"Vacacny with id {vacancyId} to get applicants for it");
                throw new Exception("Vacacny not found");
            }

            var applicants = await _vacancyRepository.GetApplicationsAsync(vacancy);
            return _mapper.Map<List<ApplicationDto>>(applicants);
        }

        public async Task ApplyForVacancyAsync(int vacancyId, string userName)
        {
            var vacancy = await _vacancyRepository.GetVacancyByIdAsync(vacancyId);

            if (vacancy == null || !vacancy.IsActive)
            {
                _logger.LogError($"Vacacny with id {vacancyId} not found or not active to applay for vacacny");

                throw new Exception("Vacancy not found or not active.");
            }


            if (vacancy.Applications != null && (vacancy.Applications.Count >= vacancy.MaxApplications))
            {
                _logger.LogError($"Maximum number of applications for this vacancy with id {vacancyId} has been reached.");
                throw new Exception("Maximum number of applications for this vacancy has been reached.");
            }

            var applicant = await _userRepository.GetUserByUsernameAsync(userName);
            int applicantId = applicant.Id;


            if (vacancy.Applications != null)
            {

                var lastApplicationDate = _vacancyRepository.GetApplicationsAsync().Result.Where
                    (a => a.UserId == applicantId)
                    .OrderByDescending(a => a.ApplicationDate)
                    .Select(a => a.ApplicationDate)
                    .FirstOrDefault();




                if (lastApplicationDate != default && lastApplicationDate >= DateTime.Now.AddHours(-24))
                {
                    _logger.LogError($"{userName} cannot apply for more than one vacancy in a 24-hour period.");
                    throw new Exception("You cannot apply for more than one vacancy in a 24-hour period.");
                }
            }



            var application = new ApplicationVacancy
            {
                VacancyId = vacancyId,
                UserId = applicantId,
                ApplicationDate = DateTime.Now
            };


            await _vacancyRepository.AddApplicationAsync(application);
            vacancy.Applications.Add(application);

            _logger.LogInformation($"applicant with id {applicantId}  applied successfully in vacancy with id {vacancyId}");


        }
        public async Task<VacancyDto> UpdateVacancy(VacancyDto vacancyDto)
        {
            var vacancy = await _vacancyRepository.GetVacancyByIdAsync(vacancyDto.Id);

            if (vacancy == null)
            {
                _logger.LogError($"Vacacny with id {vacancyDto.Id} not found to update");
                throw new Exception("Vacacny not found");
            }

            _mapper.Map(vacancyDto, vacancy);


            await _vacancyRepository.UpdateVacancyAsync(vacancy);

            _logger.LogInformation($"Vacacny with id {vacancyDto.Id} updated");
            _cache.Remove(VacancyCacheKey);
            return vacancyDto;
        }

        public async Task<PagedResult<VacancyDto>> SearchByTitleAsync(string title, int pageNumber, int pageSize)
        {
            
             var query = await _vacancyRepository.SearchByTitleAsync(title, pageNumber, pageSize);

            var vacancies =  query
          .Skip((pageNumber - 1) * pageSize)
          .Take(pageSize)
           .Select(v => new VacancyDto
           {
               Id = v.Id,
               Title = v.Title,
               Description = v.Description,
               MaxApplications = v.MaxApplications,
               ExpiryDate = v.ExpiryDate,
               IsActive = v.IsActive
           })
           .ToList();

            var totalCount =  query.Count();

            
            return new PagedResult<VacancyDto>
            {
                Results = vacancies,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
            
        }

        public async Task<IEnumerable<ApplicationVacancy>> GetApplicationsAsync()
        {

            return await _vacancyRepository.GetApplicationsAsync();
        }

        public async Task ArchiveExpiredVacanciesAsync()
        {
            var expiredVacancies = await _vacancyRepository.GetExpiredVacanciesAsync();

            foreach (var vacancy in expiredVacancies)
            {
                await _vacancyRepository.ArchiveVacancyAsync(vacancy);
                _logger.LogInformation($"Vacacny with id {vacancy.Id} archieved");
            }
        }
    }
}
