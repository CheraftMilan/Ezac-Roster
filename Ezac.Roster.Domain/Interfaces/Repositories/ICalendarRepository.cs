using Ezac.Roster.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ezac.Roster.Domain.Interfaces.Repositories
{
    public interface ICalendarRepository : IBaseRepository<Entities.Calendar>
    {
        Task<Entities.Calendar> GetByIdAsync(int id);
    }
}
