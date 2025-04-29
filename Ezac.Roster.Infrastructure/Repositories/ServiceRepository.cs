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
    public class ServiceRepository(EzacDbContext ezacDbContext, ILogger<IBaseRepository<Service>> logger) : BaseRepository<Service>(ezacDbContext, logger), IServiceRepository
    {
        public override IQueryable<Service> GetAll()
        {
            return _table.Include(s => s.RequiredQualifications).Include(s => s.PartOfDay).AsQueryable();
        }

        public async override Task<IEnumerable<Service>> GetAllAsync()
        {
            return await _table.Include(s => s.RequiredQualifications).Include(s => s.PartOfDay).ThenInclude(p => p.Day).ToListAsync();
        }

        public async Task<Service> GetByIdAsync(int id)
        {
            return await _table.Include(s => s.RequiredQualifications).ThenInclude(r => r.Member).Include(s => s.PartOfDay).FirstOrDefaultAsync(s => s.Id == id);
        }
    }
}
