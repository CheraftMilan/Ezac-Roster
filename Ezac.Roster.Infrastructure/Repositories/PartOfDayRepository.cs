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
    public class PartOfDayRepository(EzacDbContext ezacDbContext, ILogger<IBaseRepository<PartOfDay>> logger) : BaseRepository<PartOfDay>(ezacDbContext, logger), IPartOfDayRepository
    {
        public override IQueryable<PartOfDay> GetAll()
        {
            return _table.Include(p => p.Day).Include(p => p.Services).AsQueryable();
        }

        public async override Task<IEnumerable<PartOfDay>> GetAllAsync()
        {
            return await _table.Include(p => p.Day).Include(p => p.Services).ToListAsync();
        }

        public async Task<PartOfDay> GetByIdAsync(int id)
        {
            return  await _table.Include(p => p.Day).Include(p => p.Services).ThenInclude(s => s.RequiredQualifications).FirstOrDefaultAsync(p => p.Id == id);
        }
    }
}
