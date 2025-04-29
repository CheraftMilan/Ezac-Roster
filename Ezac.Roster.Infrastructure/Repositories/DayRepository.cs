using Ezac.Roster.Domain.Entities;
using Ezac.Roster.Domain.Interfaces.Repositories;
using Ezac.Roster.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ezac.Roster.Infrastructure.Repositories
{
    public class DayRepository(EzacDbContext ezacDbContext, ILogger<IBaseRepository<Day>> logger) : BaseRepository<Day>(ezacDbContext, logger), IDayRepository
    {
        public override IQueryable<Day> GetAll()
        {
            return _table.Include(d => d.PartOfDays).Include(d => d.Calendar).AsQueryable();
        }

        public async override Task<IEnumerable<Day>> GetAllAsync()
        {
            return await _table.Include(d => d.PartOfDays).Include(d => d.Calendar).ToListAsync();
        }

        public async Task<Day> GetByIdAsync(int id)
        {
            return await _table.Include(d => d.PartOfDays).Include(d => d.Calendar).FirstOrDefaultAsync(d => d.Id == id);
        }
    }
}
