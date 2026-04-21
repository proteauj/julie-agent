using Memora.Api.Data;
using Memora.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Memora.Api.Services
{
    public class ReminderService
    {
        private readonly DataContext _db;
        private readonly SmsService _smsService;

        public ReminderService(DataContext db, SmsService smsService)
        {
            _db = db;
            _smsService = smsService;
        }

        // userId est maintenant un int PARTOUT

        public async Task<List<Reminder>> GetUpcomingRemindersAsync(int userId, int hours = 24)
        {
            var now = DateTime.UtcNow;
            var end = now.AddHours(hours);
            return await _db.Reminders
                .Where(r => r.UserId == userId && r.Date >= now && r.Date <= end && !r.Done)
                .OrderBy(r => r.Date)
                .ToListAsync();
        }

        public async Task AddReminderAsync(int userId, string text, DateTime date)
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

        public async Task<List<Reminder>> GetAllRemindersAsync(int userId)
        {
            return await _db.Reminders
                .Where(r => r.UserId == userId)
                .OrderBy(r => r.Date)
                .ToListAsync();
        }

        public async Task SendRemindersDueAsync()
        {
            var now = DateTime.UtcNow;
            var dueReminders = await _db.Reminders
                .Include(r => r.User) // pour accéder au téléphone
                .Where(r => !r.Done && r.Date <= now)
                .ToListAsync();

            foreach (var reminder in dueReminders)
            {
                if (!string.IsNullOrEmpty(reminder.User?.Phone))
                {
                    // Envoie le SMS
                    _smsService.SendSms(
                        reminder.User.Phone,
                        $"Reminder: {reminder.Text} at {reminder.Date:t}"
                    );
                }
                // Option : marquer comme envoyé/done
                reminder.Done = true;
            }

            await _db.SaveChangesAsync();
        }
    }
}