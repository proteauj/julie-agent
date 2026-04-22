using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Memora.Api.Migrations
{
    /// <inheritdoc />
    public partial class FixReminderConstraints : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "Reminders",
                type: "character varying(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "general",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Channel",
                table: "Reminders",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "sms",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Reminders_UserId_ScheduledAt",
                table: "Reminders",
                columns: new[] { "UserId", "ScheduledAt" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Reminders_UserId_ScheduledAt",
                table: "Reminders");

            migrationBuilder.AlterColumn<string>(
                name: "Channel",
                table: "Reminders",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20,
                oldNullable: false,
                oldDefaultValue: "sms");

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "Reminders",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(30)",
                oldMaxLength: 30,
                oldNullable: false,
                oldDefaultValue: "general");
        }
    }
}
