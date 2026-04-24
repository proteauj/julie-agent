using BCrypt.Net;
using Memora.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Memora.Api.Data;

public static class DemoDataSeeder
{
    public static async Task SeedAsync(DataContext db)
    {
        if (await db.Users.AnyAsync())
            return;

        var residence1 = new Facility
        {
            Name = "Résidence Les Jardins d’Aline",
            Address = "123 rue Principale, Montréal"
        };

        var residence2 = new Facility
        {
            Name = "Résidence du Vieux Moulin",
            Address = "45 avenue des Érables, Longueuil"
        };

        db.Facilities.AddRange(residence1, residence2);
        await db.SaveChangesAsync();

        var admin = new User
        {
            Email = "admin@alineecoute.com",
            Nom = "Admin Aline",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password123!"),
            Role = "admin",
            LanguePreferree = "fr",
            NotificationPreference = "email",
            FacilityId = residence1.Id
        };

        var senior = new User
        {
            Email = "senior@alineecoute.com",
            Nom = "Jeannine Tremblay",
            Phone = "5145551111",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password123!"),
            Role = "senior",
            LanguePreferree = "fr",
            NotificationPreference = "both",
            FacilityId = residence1.Id
        };

        var proche = new User
        {
            Email = "proche@alineecoute.com",
            Nom = "Marie Tremblay",
            Phone = "5145552222",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password123!"),
            Role = "relative",
            LanguePreferree = "fr",
            NotificationPreference = "email"
        };

        db.Users.AddRange(admin, senior, proche);
        await db.SaveChangesAsync();

        db.Doctors.AddRange(
            new Doctor
            {
                Name = "Dre Louise Gagnon",
                Specialty = "Médecine familiale",
                BookingUrl = "https://example.com/dre-gagnon",
                FacilityId = residence1.Id
            },
            new Doctor
            {
                Name = "Dr Pierre Lavoie",
                Specialty = "Cardiologie",
                BookingUrl = "https://example.com/dr-lavoie",
                FacilityId = residence1.Id
            }
        );

        db.Activities.AddRange(
            new Activity
            {
                Name = "Bingo amical",
                Description = "Activité sociale en après-midi.",
                Start = DateTime.UtcNow.AddDays(1).Date.AddHours(18),
                MaxParticipants = 20,
                FacilityId = residence1.Id
            },
            new Activity
            {
                Name = "Atelier tricot",
                Description = "Tricot, jasette et café.",
                Start = DateTime.UtcNow.AddDays(2).Date.AddHours(19),
                MaxParticipants = 12,
                FacilityId = residence1.Id
            },
            new Activity
            {
                Name = "Musique des années 60",
                Description = "Écoute musicale et souvenirs.",
                Start = DateTime.UtcNow.AddDays(3).Date.AddHours(20),
                MaxParticipants = 25,
                FacilityId = residence2.Id
            }
        );

        db.Appointments.AddRange(
            new Appointment
            {
                UserId = senior.Id,
                Title = "Rendez-vous médical",
                Description = "Suivi annuel",
                Start = DateTime.UtcNow.AddDays(4).Date.AddHours(14),
                End = DateTime.UtcNow.AddDays(4).Date.AddHours(15),
                Type = "medical"
            },
            new Appointment
            {
                UserId = senior.Id,
                Title = "Dîner avec Marie",
                Description = "Dîner avec ma fille",
                Start = DateTime.UtcNow.AddDays(5).Date.AddHours(16),
                End = DateTime.UtcNow.AddDays(5).Date.AddHours(17),
                Type = "personal"
            }
        );

        db.Reminders.AddRange(
            new Reminder
            {
                UserId = senior.Id,
                Title = "Prendre médicament",
                Description = "Médicament du matin",
                ScheduledAt = DateTime.UtcNow.AddHours(2),
                Type = "medication",
                Channel = "both",
                IsDone = false
            },
            new Reminder
            {
                UserId = senior.Id,
                Title = "Appeler Marie",
                Description = "Prendre des nouvelles",
                ScheduledAt = DateTime.UtcNow.AddDays(1).Date.AddHours(21),
                Type = "general",
                Channel = "sms",
                IsDone = false
            }
        );

        await db.SaveChangesAsync();
    }
}