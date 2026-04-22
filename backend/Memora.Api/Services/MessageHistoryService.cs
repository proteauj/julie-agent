using Memora.Api.Data;
using Memora.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Memora.Api.Services
{
    public class MessageHistoryService
    {
        private readonly DataContext _db;

        public MessageHistoryService(DataContext db)
        {
            _db = db;
        }

        public async Task AddMessageAsync(int userId, string role, string text)
        {
            var msg = new Message
            {
                UserId = userId,
                Role = role,
                Content = text,            
                CreatedAt = DateTime.UtcNow 
            };

            _db.Messages.Add(msg);
            await _db.SaveChangesAsync();
        }

        public async Task<List<Message>> GetLastMessagesAsync(int userId, int count = 6)
        {
            return await _db.Messages
                .Where(m => m.UserId == userId)
                .OrderByDescending(m => m.CreatedAt) // ✅ FIX
                .Take(count)
                .OrderBy(m => m.CreatedAt)           // ✅ FIX
                .ToListAsync();
        }
    }
}