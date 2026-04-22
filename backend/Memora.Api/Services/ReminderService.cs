using Memora.Api.Data;
using Memora.Api.Dtos;
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

        public async Task<List<Reminder>> GetUpcomingRemindersAsync(int userId, int hours = 24)
        {
            var now = DateTime.UtcNow;
            var end = now.AddHours(hours);

            return await _db.Reminders
                .Where(r => r.UserId == userId && r.ScheduledAt >= now && r.ScheduledAt <= end && !r.IsDone)
                .OrderBy(r => r.ScheduledAt)
                .ToListAsync();
        }

        public async Task<List<Reminder>> GetAllRemindersAsync(int userId)
        {
            return await _db.Reminders
                .Where(r => r.UserId == userId)
                .OrderBy(r => r.ScheduledAt)
                .ToListAsync();
        }

        public async Task<Reminder?> GetReminderByIdAsync(int reminderId, int userId)
        {
            return await _db.Reminders
                .FirstOrDefaultAsync(r => r.Id == reminderId && r.UserId == userId);
        }

        public async Task<Reminder> CreateReminderAsync(int userId, CreateReminderDto dto)
        {
            var reminder = new Reminder
            {
                UserId = userId,
                Title = dto.Title,
                Description = dto.Description,
                ScheduledAt = dto.ScheduledAt,
                Type = dto.Type,
                Channel = dto.Channel,
                IsRecurring = dto.IsRecurring,
                RecurrenceRule = dto.RecurrenceRule,
                SourceId = dto.SourceId,
                SourceType = dto.SourceType,
                IsDone = false,
                CreatedAt = DateTime.UtcNow
            };

            _db.Reminders.Add(reminder);
            await _db.SaveChangesAsync();

            return reminder;
        }

        public async Task<Reminder?> UpdateReminderAsync(int reminderId, int userId, UpdateReminderDto dto)
        {
            var reminder = await _db.Reminders
                .FirstOrDefaultAsync(r => r.Id == reminderId && r.UserId == userId);

            if (reminder == null)
                return null;

            reminder.Title = dto.Title;
            reminder.Description = dto.Description;
            reminder.ScheduledAt = dto.ScheduledAt;
            reminder.Type = dto.Type;
            reminder.Channel = dto.Channel;
            reminder.IsRecurring = dto.IsRecurring;
            reminder.RecurrenceRule = dto.RecurrenceRule;
            reminder.SourceId = dto.SourceId;
            reminder.SourceType = dto.SourceType;
            reminder.IsDone = dto.IsDone;
            reminder.CompletedAt = dto.IsDone ? (reminder.CompletedAt ?? DateTime.UtcNow) : null;

            await _db.SaveChangesAsync();
            return reminder;
        }

        public async Task<bool> DeleteReminderAsync(int reminderId, int userId)
        {
            var reminder = await _db.Reminders
                .FirstOrDefaultAsync(r => r.Id == reminderId && r.UserId == userId);

            if (reminder == null)
                return false;

            _db.Reminders.Remove(reminder);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<Reminder?> MarkReminderDoneAsync(int reminderId, int userId)
        {
            var reminder = await _db.Reminders
                .FirstOrDefaultAsync(r => r.Id == reminderId && r.UserId == userId);

            if (reminder == null)
                return null;

            reminder.IsDone = true;
            reminder.CompletedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();
            return reminder;
        }

        public async Task SendRemindersDueAsync()
        {
            var now = DateTime.UtcNow;

            var dueReminders = await _db.Reminders
                .Include(r => r.User)
                .Where(r => !r.IsDone && r.ScheduledAt <= now)
                .OrderBy(r => r.ScheduledAt)
                .ToListAsync();

            foreach (var reminder in dueReminders)
            {
                var userPreference = reminder.User?.NotificationPreference?.ToLowerInvariant() ?? "sms";
                var effectiveChannel = string.IsNullOrWhiteSpace(reminder.Channel)
                    ? userPreference
                    : reminder.Channel.ToLowerInvariant();

                var message = BuildReminderMessage(reminder);

                if ((effectiveChannel == "sms" || effectiveChannel == "both")
                    && !string.IsNullOrWhiteSpace(reminder.User?.Phone))
                {
                    _smsService.SendSms(reminder.User!.Phone!, message);
                }

                // email à venir en phase suivante
                // if (effectiveChannel == "email" || effectiveChannel == "both") { ... }

                reminder.IsDone = true;
                reminder.CompletedAt = DateTime.UtcNow;

                if (reminder.IsRecurring)
                {
                    var nextDate = GetNextOccurrence(reminder.ScheduledAt, reminder.RecurrenceRule);

                    if (nextDate.HasValue)
                    {
                        var nextReminder = new Reminder
                        {
                            UserId = reminder.UserId,
                            Title = reminder.Title,
                            Description = reminder.Description,
                            ScheduledAt = nextDate.Value,
                            Type = reminder.Type,
                            Channel = reminder.Channel,
                            IsRecurring = reminder.IsRecurring,
                            RecurrenceRule = reminder.RecurrenceRule,
                            SourceId = reminder.SourceId,
                            SourceType = reminder.SourceType,
                            IsDone = false,
                            CreatedAt = DateTime.UtcNow
                        };

                        _db.Reminders.Add(nextReminder);
                    }
                }
            }

            await _db.SaveChangesAsync();
        }

        private static string BuildReminderMessage(Reminder reminder)
        {
            var when = reminder.ScheduledAt.ToLocalTime().ToString("g");
            return $"Rappel: {reminder.Title} - {when}";
        }

        private static DateTime? GetNextOccurrence(DateTime currentDate, string? recurrenceRule)
        {
            return recurrenceRule?.ToLowerInvariant() switch
            {
                "daily" => currentDate.AddDays(1),
                "weekly" => currentDate.AddDays(7),
                "monthly" => currentDate.AddMonths(1),
                _ => null
            };
        }

        public static ReminderDto ToDto(Reminder reminder)
        {
            return new ReminderDto
            {
                Id = reminder.Id,
                UserId = reminder.UserId,
                Title = reminder.Title,
                Description = reminder.Description,
                ScheduledAt = reminder.ScheduledAt,
                Type = reminder.Type,
                Channel = reminder.Channel,
                IsDone = reminder.IsDone,
                CompletedAt = reminder.CompletedAt,
                IsRecurring = reminder.IsRecurring,
                RecurrenceRule = reminder.RecurrenceRule,
                SourceId = reminder.SourceId,
                SourceType = reminder.SourceType,
                CreatedAt = reminder.CreatedAt
            };
        }
    }
}