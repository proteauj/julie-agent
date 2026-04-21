using JulieAgent.Api.Data;
using JulieAgent.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace JulieAgent.Api.Services
{
    public class RappelService
    {
        private readonly DataContext _db;
        public RappelService(DataContext db) { _db = db; }

        public async Task<List<Rappel>> GetNextRappelsAsync(string userId)
        {
            var now = DateTime.UtcNow;
            return await _db.Rappels.Where(r => r.UserId == userId && r.Date >= now && !r.Fait)
                       .OrderBy(r => r.Date).ToListAsync();
        }
    }
}