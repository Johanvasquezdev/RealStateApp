using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RealEstateApp.Infrastructure.Identity.Migrations
{
    /// <inheritdoc />
    public partial class RemoveIdCard : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IdCard",
                schema: "Identity",
                table: "Users");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "IdCard",
                schema: "Identity",
                table: "Users",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
