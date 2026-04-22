using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Memora.Api.Migrations
{
    /// <inheritdoc />
    public partial class UpdateReminderModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Ajouter nouvelles colonnes
            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "Reminders",
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ScheduledAt",
                table: "Reminders",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Reminders",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "Reminders",
                maxLength: 30,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Channel",
                table: "Reminders",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDone",
                table: "Reminders",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Reminders",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP");

            // 🔁 Migration des données existantes
            migrationBuilder.Sql(@"
                UPDATE ""Reminders""
                SET ""Title"" = ""Text"",
                    ""ScheduledAt"" = ""Date"",
                    ""IsDone"" = ""Done"",
                    ""Type"" = 'general',
                    ""Channel"" = 'sms'
            ");

            // Supprimer anciennes colonnes
            migrationBuilder.DropColumn(name: "Text", table: "Reminders");
            migrationBuilder.DropColumn(name: "Date", table: "Reminders");
            migrationBuilder.DropColumn(name: "Done", table: "Reminders");

            // Rendre NOT NULL après migration
            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Reminders",
                maxLength: 150,
                nullable: false);

            migrationBuilder.AlterColumn<DateTime>(
                name: "ScheduledAt",
                table: "Reminders",
                nullable: false);
        }

        /// <inheritdoc />
       protected override void Down(MigrationBuilder migrationBuilder)
        {
            // 🔙 Recréer anciennes colonnes
            migrationBuilder.AddColumn<string>(
                name: "Text",
                table: "Reminders",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Date",
                table: "Reminders",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Done",
                table: "Reminders",
                nullable: false,
                defaultValue: false);

            // 🔁 Re-mapper les données
            migrationBuilder.Sql(@"
                UPDATE ""Reminders""
                SET ""Text"" = ""Title"",
                    ""Date"" = ""ScheduledAt"",
                    ""Done"" = ""IsDone""
            ");

            // ❌ Supprimer nouvelles colonnes
            migrationBuilder.DropColumn(name: "Title", table: "Reminders");
            migrationBuilder.DropColumn(name: "Description", table: "Reminders");
            migrationBuilder.DropColumn(name: "ScheduledAt", table: "Reminders");
            migrationBuilder.DropColumn(name: "Type", table: "Reminders");
            migrationBuilder.DropColumn(name: "Channel", table: "Reminders");
            migrationBuilder.DropColumn(name: "IsDone", table: "Reminders");
            migrationBuilder.DropColumn(name: "CompletedAt", table: "Reminders");
            migrationBuilder.DropColumn(name: "IsRecurring", table: "Reminders");
            migrationBuilder.DropColumn(name: "RecurrenceRule", table: "Reminders");
            migrationBuilder.DropColumn(name: "SourceId", table: "Reminders");
            migrationBuilder.DropColumn(name: "SourceType", table: "Reminders");
            migrationBuilder.DropColumn(name: "CreatedAt", table: "Reminders");
        }
    }
}
