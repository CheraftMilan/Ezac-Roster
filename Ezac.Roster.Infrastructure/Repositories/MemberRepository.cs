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
    public class MemberRepository(EzacDbContext ezacDbContext, ILogger<IBaseRepository<Member>> logger) : BaseRepository<Member>(ezacDbContext, logger), IMemberRepository
    {
        public async Task<bool> DeleteAllAsync()
        {
            //delete all members
            var members = _table.ToList();
            _table.RemoveRange(members);
            return await SaveChangesAsync();
        }

        public override IQueryable<Member> GetAll()
        {
            return _table.Include(m => m.PlannedServices).Include(m => m.Qualifications).Include(m => m.Preferences).AsQueryable();
        }

        public async override Task<IEnumerable<Member>> GetAllAsync()
        {
            return await _table.Include(m => m.PlannedServices).Include(m => m.Qualifications).Include(m => m.Preferences).ToListAsync();
        }

        public async Task<Member> GetByIdAsync(string id)
        {
            return await _table.Include(m => m.PlannedServices).Include(m => m.Qualifications).ThenInclude(m => m.Services).FirstOrDefaultAsync(m => m.Id == id);
        }
        public async Task<bool> GiveQualificationToMember(string memberId, int qualificationId)
        {
            var member = await _table.Include(m => m.Qualifications).FirstOrDefaultAsync(m => m.Id == memberId);
            var qualification = await _ezacDbContext.Qualifications.FirstOrDefaultAsync(q => q.Id == qualificationId);
            member.Qualifications.Add(qualification);
            return await SaveChangesAsync();
            
        }
        public async Task<bool> RemoveMemberByCalendarId(int calendarId)
        {
            var members = await _table.Where(m => m.CalendarId == calendarId).ToListAsync();
            _table.RemoveRange(members);
            var memberIds = members.Select(m => m.Id);
            var qualifications = _ezacDbContext.Qualifications.Where(q => memberIds.Contains(q.MemberId));
            _ezacDbContext.Qualifications.RemoveRange(qualifications);
            var preferences = _ezacDbContext.Preferences.Where(q => memberIds.Contains(q.MemberId));
            _ezacDbContext.Preferences.RemoveRange(preferences);
            return await SaveChangesAsync();
        }
    }
}
