using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Honeys_Kitchen_backend.Migrations
{
    /// <inheritdoc />
    public partial class ModelUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "EmailAddress",
                table: "User",
                newName: "Email");

            migrationBuilder.RenameIndex(
                name: "IX_User_EmailAddress",
                table: "User",
                newName: "IX_User_Email");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Email",
                table: "User",
                newName: "EmailAddress");

            migrationBuilder.RenameIndex(
                name: "IX_User_Email",
                table: "User",
                newName: "IX_User_EmailAddress");
        }
    }
}
