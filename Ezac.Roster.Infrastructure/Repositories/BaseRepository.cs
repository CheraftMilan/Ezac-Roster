using Ezac.Roster.Domain.Interfaces.Repositories;
using Ezac.Roster.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ezac.Roster.Infrastructure.Repositories
{
    public class BaseRepository<T> : IBaseRepository<T> where T : class
    {
        protected readonly EzacDbContext _ezacDbContext;
        protected readonly ILogger<IBaseRepository<T>> _logger;
        protected readonly DbSet<T> _table;
        public BaseRepository(EzacDbContext ezacDbContext, ILogger<IBaseRepository<T>> logger)
        {
            _ezacDbContext = ezacDbContext;
            _logger = logger;
            _table = _ezacDbContext.Set<T>();
        }

        public async Task<bool> AddAsync(T toAdd)
        {
            _table.Add(toAdd);
            return await SaveChangesAsync();
        }

        public virtual async Task<bool> DeleteAsync(T toDelete)
        {
            _table.Remove(toDelete);
            return await SaveChangesAsync();
        }

        public virtual IQueryable<T> GetAll()
        {
            return _table.AsQueryable();
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _table.ToListAsync();
        }

        public async Task<bool> UpdateAsync(T toUpdate)
        {
            _table.Update(toUpdate);
            return await SaveChangesAsync();
        }

        protected async Task<bool> SaveChangesAsync()
        {
            try
            {
                await _ezacDbContext.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateException dbUpdateException)
            {
                _logger.LogError(dbUpdateException.Message);
                return false;
            }
        }
    }
}
