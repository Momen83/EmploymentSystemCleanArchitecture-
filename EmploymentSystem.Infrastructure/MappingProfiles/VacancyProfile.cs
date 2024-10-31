using AutoMapper;
using EmploymentSystem.Application.Dtos;
using EmploymentSystem.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmploymentSystem.Infrastructure.MappingProfiles
{
    public class VacancyProfile : Profile
    {
        public VacancyProfile()
        {

            CreateMap<Vacancy, VacancyDto>();

            CreateMap<VacancyDto, Vacancy>()
                .ForMember(dest => dest.Employer, opt => opt.Ignore())
                .ForMember(dest => dest.Applications, opt => opt.Ignore());
        }
    }
}
