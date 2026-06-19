using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedlinkDialysisCenter.Migrations
{
    /// <inheritdoc />
    public partial class AddHepaTestOneToOne : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HepaTests",
                columns: table => new
                {
                    HepaTestId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PatientId = table.Column<int>(type: "int", nullable: false),
                    HepaBTested = table.Column<bool>(type: "bit", nullable: false),
                    HepaBResult = table.Column<int>(type: "int", nullable: true),
                    AntiHBSTested = table.Column<bool>(type: "bit", nullable: false),
                    AntiHBSResult = table.Column<int>(type: "int", nullable: true),
                    HepaCTested = table.Column<bool>(type: "bit", nullable: false),
                    HepaCResult = table.Column<int>(type: "int", nullable: true),
                    TestedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HepaTests", x => x.HepaTestId);
                    table.ForeignKey(
                        name: "FK_HepaTests_Patients_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Patients",
                        principalColumn: "PatientId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HepaTests_PatientId",
                table: "HepaTests",
                column: "PatientId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HepaTests");
        }
    }
}
