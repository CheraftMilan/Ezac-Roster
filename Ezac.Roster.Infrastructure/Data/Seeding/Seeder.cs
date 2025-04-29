using Ezac.Roster.Domain.Entities;
using Ezac.Roster.Domain.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Runtime.Serialization.DataContracts;

namespace Ezac.Roster.Infrastructure.Data.Seeding
{
    public class Seeder
    {
        public static void Seed(ModelBuilder modelBuilder)
        {
            var adminUsers = new AdminUser
            {
                Id = "1",
                Name = "Administrator",
                UserName = "Admin",
                NormalizedUserName = "ADMIN"
            };
            IPasswordHasher<AdminUser> passwordHasher = new PasswordHasher<AdminUser>();
            adminUsers.PasswordHash = passwordHasher.HashPassword(adminUsers, "Test123?");

            var qualifications = new Qualification[] {
                new() { Id = 1, Name = "Lierist"},
                new() { Id = 2, Name = "Startofficier"},
                new() { Id = 3, Name = "Instructeur"},
                new() { Id = 4, Name = "Bar"}
            };

            var services = new Service[] {
                new() { Id = 1, Name = "Lieren", Weight = 5,RequeredExperience = 1 },
                new() { Id = 2, Name = "Bar", Weight = 2, RequeredExperience = 1},
                new() { Id = 3, Name = "Startofficier", Weight = 7, RequeredExperience = 1},
                new() { Id = 4, Name = "Instructeur", Weight = 5, RequeredExperience = 1},
                new() { Id = 5, Name = "DDI Instructeur", Weight = 5, RequeredExperience = 1},
            };

            modelBuilder.Entity<AdminUser>().HasData(adminUsers);
            modelBuilder.Entity<Qualification>().HasData(qualifications);
            modelBuilder.Entity<Service>().HasData(services);

            modelBuilder.Entity($"{nameof(Qualification)}{nameof(Service)}").HasData(
                new { QualificationId = 1, ServicesId = 1, RequiredQualificationsId = 1 },
                new { QualificationId = 2, ServicesId = 3, RequiredQualificationsId = 2  },
                new { QualificationId = 3, ServicesId = 4, RequiredQualificationsId = 3 },
                new { QualificationId = 3, ServicesId = 5, RequiredQualificationsId = 3 },
                new { QualificationId = 4, ServicesId = 2, RequiredQualificationsId = 4 }
            );
        }
    }
}
