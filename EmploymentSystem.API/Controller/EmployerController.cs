using EmploymentSystem.Application.Dtos;
using EmploymentSystem.Application.Services;
using EmploymentSystem.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmploymentSystem.API.Controller
{
    [ApiController]
    [Route("api/vacancy")]
    [Authorize(Roles = "Employer")]
    public class EmployerController : ControllerBase
    {
        private readonly IVacancyService _vacancyService;

        public EmployerController(IVacancyService vacancyService)
        {
            _vacancyService = vacancyService;
        }



        [HttpGet]
        public async Task<ActionResult<IEnumerable<VacancyDto>>> GetVacancies()
        {
            var vacancies = await _vacancyService.GetVacancies();
            return Ok(vacancies);
        }



        [HttpGet("applicants/{id}")]
        public async Task<ActionResult<IEnumerable<ApplicationDto>>> GetVacancyApplicants(int id)
        {
            try
            {
                var applicants = await _vacancyService.GetVacancyApplicants(id);
                return Ok(applicants);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public async Task<ActionResult<VacancyDto>> CreateVacancy([FromBody] VacancyDto vacancyDto)
        {
            if (vacancyDto == null)
            {
                return BadRequest("Vacancy data is required.");
            }

            try
            {
                var userName = User.Identity.Name;
                var createdVacancy = await _vacancyService.CreateVacancy(vacancyDto, userName);
                createdVacancy.IsActive = true;

                return CreatedAtAction(nameof(GetVacancies), new { id = createdVacancy.Id }, createdVacancy);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("status/{id}")]
        public async Task<IActionResult> ChangeVacancyStatus(int id)
        {
            try
            {
                await _vacancyService.ChangeVacancyStatus(id);
                return Ok("Vacancy Status changed Successfully");
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }

        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVacancy(int id)
        {
            try
            {
                await _vacancyService.DeleteVacancy(id);
                return Ok("Vacancy Deleted  Successfully");
            }


            catch (Exception ex)
            {
                return NotFound(ex.Message);

            }
        }

        [HttpPut]
        public async Task<ActionResult<VacancyDto>> UpdateVacancy([FromBody] VacancyDto vacancyDto)
        {
            if (vacancyDto == null)
            {
                return BadRequest("Vacancy data is required.");
            }

            try
            {
                var updatedVacancy = await _vacancyService.UpdateVacancy(vacancyDto);
                return Ok(updatedVacancy);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
