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
    public class QualificationRepository(EzacDbContext ezacDbContext, ILogger<IBaseRepository<Qualification>> logger) : BaseRepository<Qualification>(ezacDbContext, logger), IQualificationRepository
    {
        public override IQueryable<Qualification> GetAll()
        {
            return _table.Include(q => q.Member).Include(q => q.Services).AsQueryable();
        }

        public async override Task<IEnumerable<Qualification>> GetAllAsync()
        {
            return await _table.Include(q => q.Member).Include(q => q.Services).ToListAsync();
        }

        public async Task<Qualification> GetByIdAsync(int id)
        {
            return await _table.Include(q => q.Member).Include(q => q.Services).FirstOrDefaultAsync(q => q.Id == id);
        }
    }
}
