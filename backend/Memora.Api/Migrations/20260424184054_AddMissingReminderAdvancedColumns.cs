using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Memora.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddMissingReminderAdvancedColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CompletedAt",
                table: "Reminders",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsRecurring",
                table: "Reminders",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "RecurrenceRule",
                table: "Reminders",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SourceId",
                table: "Reminders",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SourceType",
                table: "Reminders",
                type: "character varying(30)",
                maxLength: 30,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CompletedAt",
                table: "Reminders");

            migrationBuilder.DropColumn(
                name: "IsRecurring",
                table: "Reminders");

            migrationBuilder.DropColumn(
                name: "RecurrenceRule",
                table: "Reminders");

            migrationBuilder.DropColumn(
                name: "SourceId",
                table: "Reminders");

            migrationBuilder.DropColumn(
                name: "SourceType",
                table: "Reminders");
        }
    }
}
