using Ezac.Roster.Domain.Entities;
using Ezac.Roster.Domain.Models.Preference;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ezac.Roster.Domain.Interfaces.Repositories
{
    public interface IPreferenceRepository : IBaseRepository<Preference>
    {
        Task<Preference> GetByIdAsync(int id);
    }
}
