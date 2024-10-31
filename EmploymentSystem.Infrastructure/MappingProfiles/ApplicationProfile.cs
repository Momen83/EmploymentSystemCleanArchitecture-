using AutoMapper;
using EmploymentSystem.Application.Dtos;
using EmploymentSystem.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace EmploymentSystem.Infrastructure.MappingProfiles
{
    public class ApplicationProfile : Profile
    {
        public ApplicationProfile()
        {

            CreateMap<ApplicationVacancy, ApplicationDto>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Applicant.Username))
                .ForMember(dest => dest.VacacnyTitle, opt => opt.MapFrom(src => src.Vacancy.Title))
                .ForMember(dest => dest.VacancyId, opt => opt.MapFrom(src => src.VacancyId));
            CreateMap<ApplicationDto, ApplicationVacancy>();
        }
    }
}
