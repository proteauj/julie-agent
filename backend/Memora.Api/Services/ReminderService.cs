using Memora.Api.Data;
using Memora.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Memora.Api.Services
{
    public class ReminderService
    {
        private readonly DataContext _db;
        public ReminderService(DataContext db)
        {
            _db = db;
        }

        public async Task<List<Reminder>> GetUpcomingRemindersAsync(string userId, int hours = 24)
        {
            var now = DateTime.UtcNow;
            var end = now.AddHours(hours);
            return await _db.Reminders
                .Where(r => r.UserId == userId && r.Date >= now && r.Date <= end && !r.Done)
                .OrderBy(r => r.Date)
                .ToListAsync();
        }

        public async Task AddReminderAsync(string userId, string text, DateTime date)
        {
            var reminder = new Reminder
            {
                UserId = userId,
                Text = text,
                Date = date,
                Done = false
            };
            _db.Reminders.Add(reminder);
            await _db.SaveChangesAsync();
        }

        public async Task MarkReminderDoneAsync(int reminderId)
        {
            var reminder = await _db.Reminders.FindAsync(reminderId);
            if (reminder != null)
            {
                reminder.Done = true;
                await _db.SaveChangesAsync();
            }
        }

        public async Task<List<Reminder>> GetAllRemindersAsync(string userId)
        {
            return await _db.Reminders
                .Where(r => r.UserId == userId)
                .OrderBy(r => r.Date)
                .ToListAsync();
        }
    }
}