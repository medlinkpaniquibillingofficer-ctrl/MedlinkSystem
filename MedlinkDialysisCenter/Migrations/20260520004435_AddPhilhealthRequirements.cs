using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedlinkDialysisCenter.Migrations
{
    /// <inheritdoc />
    public partial class AddPhilhealthRequirements : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PhilhealthRequirements",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PatientId = table.Column<int>(type: "int", nullable: false),
                    MemberCategory = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    HasCSF = table.Column<bool>(type: "bit", nullable: false),
                    HasCF2 = table.Column<bool>(type: "bit", nullable: false),
                    HasMDR = table.Column<bool>(type: "bit", nullable: false),
                    HasPhilhealthId = table.Column<bool>(type: "bit", nullable: false),
                    HasReceipt6Mos = table.Column<bool>(type: "bit", nullable: false),
                    HasCertMonthlyContrib = table.Column<bool>(type: "bit", nullable: false),
                    HasSCId = table.Column<bool>(type: "bit", nullable: false),
                    HasCSFEmployerSig = table.Column<bool>(type: "bit", nullable: false),
                    HasPDDRegistration = table.Column<bool>(type: "bit", nullable: false),
                    HasPhilhealthConsumption = table.Column<bool>(type: "bit", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CPNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhilhealthRequirements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PhilhealthRequirements_Patients_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Patients",
                        principalColumn: "PatientId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PhilhealthRequirements_PatientId",
                table: "PhilhealthRequirements",
                column: "PatientId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PhilhealthRequirements");
        }
    }
}
