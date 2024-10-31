using EmploymentSystem.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmploymentSystem.Infrastructure.Data
{
    public class EmploymentDbContext : DbContext
    {
        public EmploymentDbContext(DbContextOptions<EmploymentDbContext> options) : base(options)
        {

        }
        public DbSet<Vacancy> Vacancies { get; set; }
        public DbSet<ApplicationVacancy> Applications { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.Id);
                entity.Property(u => u.Username)
                    .IsRequired()
                    .HasMaxLength(50);
                entity.Property(u => u.Password)
                    .IsRequired()
                    .HasMaxLength(100);
                entity.Property(u => u.Role)
                    .IsRequired()
                    .HasMaxLength(20);
            });

            modelBuilder.Entity<Vacancy>(entity =>
            {
                entity.HasKey(v => v.Id);
                entity.Property(v => v.Title)
                    .IsRequired()
                    .HasMaxLength(100);
                entity.Property(v => v.Description)
                    .HasMaxLength(500);
                entity.Property(v => v.ExpiryDate)
                    .IsRequired();
                entity.Property(v => v.MaxApplications)
                    .IsRequired();
                entity.Property(v => v.IsActive)
                    .IsRequired()
                    .HasDefaultValue(true);

                entity.HasMany(v => v.Applications)
                    .WithOne(a => a.Vacancy)
                    .HasForeignKey(a => a.VacancyId)
                    .OnDelete(DeleteBehavior.Cascade);
            });



            modelBuilder.Entity<ApplicationVacancy>(entity =>
            {
                entity.HasKey(a => a.Id);
                entity.Property(a => a.ApplicationDate)
                    .IsRequired();


                entity.HasOne(a => a.Applicant)
                    .WithMany()
                    .HasForeignKey(a => a.UserId)
                    .OnDelete(DeleteBehavior.Restrict);


                entity.HasOne(a => a.Vacancy)
                    .WithMany(v => v.Applications)
                    .HasForeignKey(a => a.VacancyId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }




    }
}
