using Ezac.Roster.Domain.Entities;
using Ezac.Roster.Domain.Interfaces.Repositories;
using Ezac.Roster.Domain.Models.Preference;
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

    public class PreferenceRepository(EzacDbContext ezacDbContext, ILogger<IBaseRepository<Preference>> logger) : BaseRepository<Preference>(ezacDbContext, logger), IPreferenceRepository
    {
        public Task<Preference> GetByIdAsync(int id)
        {
            return _table.Include(p => p.Member).Include(p => p.Service).Include(p => p.PartOfDay).FirstOrDefaultAsync(p => p.Id == id);
        }

       
    }
}
