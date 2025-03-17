using EmploymentSystem.Application.Services;
using EmploymentSystem.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmploymentSystem.API.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Applicant")]
    public class ApplicantController : ControllerBase
    {

        private readonly IVacancyService _vacancyService;

        public ApplicantController(IVacancyService vacancyService)
        {
            _vacancyService = vacancyService;
        }

        [HttpPost("apply/{vacancyId}")]
        public async Task<ActionResult> ApplyForVacancy(int vacancyId)
        {
            try
            {
                var userName = User.Identity.Name;
                await _vacancyService.ApplyForVacancyAsync(vacancyId, userName);
                return Ok("Application submitted successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchVacancies(string title = "", int pageNumber = 1, int pageSize = 10)
        {
            var result = await _vacancyService.SearchByTitleAsync(title, pageNumber, pageSize);
            return Ok(result);
        }

    }
}
