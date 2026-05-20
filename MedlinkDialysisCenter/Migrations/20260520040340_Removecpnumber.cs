using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedlinkDialysisCenter.Migrations
{
    /// <inheritdoc />
    public partial class Removecpnumber : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CPNumber",
                table: "PHRequirements");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CPNumber",
                table: "PHRequirements",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);
        }
    }
}
