using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Memora.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddReminderAdvancedFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Text",
                table: "Messages",
                newName: "Content");

            migrationBuilder.RenameColumn(
                name: "SentAt",
                table: "Messages",
                newName: "CreatedAt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "Messages",
                newName: "SentAt");

            migrationBuilder.RenameColumn(
                name: "Content",
                table: "Messages",
                newName: "Text");
        }
    }
}
