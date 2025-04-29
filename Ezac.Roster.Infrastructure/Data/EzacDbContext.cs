using Ezac.Roster.Domain.Entities;
using Ezac.Roster.Infrastructure.Data.Seeding;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ezac.Roster.Infrastructure.Data
{
    public class EzacDbContext(DbContextOptions<EzacDbContext> options) : IdentityDbContext<AdminUser>(options)
    {
        public DbSet<AdminUser> AdminUsers { get; set; }
        public DbSet<Member> Members { get; set; }
        public DbSet<Qualification> Qualifications { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<PartOfDay> PartOfDays { get; set; }
        public DbSet<Day> Days { get; set; }
        public DbSet<Calendar> Calendars { get; set; }


        public DbSet<Preference> Preferences { get; set; }
        public DbSet<PlannedService> PlannedServices { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            Seeder.Seed(modelBuilder);
            base.OnModelCreating(modelBuilder);
        }
    }
}
