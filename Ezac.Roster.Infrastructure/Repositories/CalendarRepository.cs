using Ezac.Roster.Domain.Entities;
using Ezac.Roster.Domain.Interfaces.Repositories;
using Ezac.Roster.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ezac.Roster.Infrastructure.Repositories
{
    public class CalendarRepository(EzacDbContext ezacDbContext, ILogger<IBaseRepository<Calendar>> logger) : BaseRepository<Calendar>(ezacDbContext, logger), ICalendarRepository
    {
        public override IQueryable<Calendar> GetAll()
        {
            return _table.Include(c => c.Days).AsQueryable();
        }
        public override async Task<bool> DeleteAsync(Calendar toDelete)
        {
            var members = await _ezacDbContext.Members.Where(m => m.CalendarId == toDelete.Id).ToListAsync();
            var qualificationFromMembers = await _ezacDbContext.Qualifications.Where(q => members.Select(m => m.Id).Contains(q.MemberId)).ToListAsync();
            var preferences = await _ezacDbContext.Preferences.Where(p => p.CalendarId == toDelete.Id).ToListAsync();
            var plannedServices = await _ezacDbContext.PlannedServices.Where(p => p.CalendarId == toDelete.Id).ToListAsync();
            var days = await _ezacDbContext.Days.Where(d => d.CalendarId == toDelete.Id).ToListAsync();
            var partOfDays = await _ezacDbContext.PartOfDays.Where(p => days.Select(d => d.Id).Contains(p.DayId)).ToListAsync();
            var partOfDayIds = partOfDays.Select(p => p.Id).ToList();
            var services = await _ezacDbContext.Services
                .Where(s => partOfDayIds.Contains(s.PartOfDayId.Value))
                .ToListAsync();
            _table.Remove(toDelete);
            _ezacDbContext.Qualifications.RemoveRange(qualificationFromMembers);
            _ezacDbContext.PlannedServices.RemoveRange(plannedServices);
            _ezacDbContext.Members.RemoveRange(members);
            _ezacDbContext.Preferences.RemoveRange(preferences);
            _ezacDbContext.Days.RemoveRange(days);
            _ezacDbContext.PartOfDays.RemoveRange(partOfDays);
            _ezacDbContext.Services.RemoveRange(services);
            return await SaveChangesAsync();
        }
        public async override Task<IEnumerable<Calendar>> GetAllAsync()
        {
            return await _table.Include(c => c.Days).ThenInclude(c => c.PartOfDays).ToListAsync();
        }

        public async Task<Calendar> GetByIdAsync(int id)
        {
            return await _table.Include(c => c.Days).ThenInclude(c => c.PartOfDays).ThenInclude(p => p.Services).Include(c => c.Members).FirstOrDefaultAsync(c => c.Id == id);
        }
    }
}
