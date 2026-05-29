using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedlinkDialysisCenter.Migrations
{
    /// <inheritdoc />
    public partial class AddPatientCode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PatientCode",
                table: "Patients",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PatientCode",
                table: "Patients");
        }
    }
}
