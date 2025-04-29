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
    public class PlannedServiceRepository(EzacDbContext ezacDbContext, ILogger<IBaseRepository<PlannedService>> logger) : BaseRepository<PlannedService>(ezacDbContext, logger), IPlannedServiceRepository
    {
        public override IQueryable<PlannedService> GetAll()
        {
            return _table.Include(p => p.Service).Include(p => p.Member).AsQueryable();
        }

        public async override Task<IEnumerable<PlannedService>> GetAllAsync()
        {
            return await _table.Include(p => p.Service).Include(p => p.Member).ToListAsync();
        }

        public async Task<PlannedService> GetByIdAsync(int id)
        {
            return await _table.Include(p => p.Service).Include(p => p.Member).FirstOrDefaultAsync(p => p.Id == id);
        }
        public async Task<bool> DeleteByCalendarIdAsync(int calendarId)
        {
            var plannedServices = await _table.Where(p => p.CalendarId == calendarId).ToListAsync();
            _table.RemoveRange(plannedServices);
            return await SaveChangesAsync();
        }
    }
}
