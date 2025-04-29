using Ezac.Roster.Domain.Entities;
using Ezac.Roster.Infrastructure.Models.Result;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ezac.Roster.Domain.Interfaces.Services
{
    public interface IGenerateService
    {
        Task<ResultModel<IEnumerable<PlannedService>>> GenerateCalendar(int calendarId);
    }
}
