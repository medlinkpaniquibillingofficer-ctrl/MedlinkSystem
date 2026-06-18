using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedlinkDialysisCenter.Migrations
{
    /// <inheritdoc />
    public partial class AddExpiryBatchTracking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BatchNumber",
                table: "StockTransactions",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ExpiryDate",
                table: "StockTransactions",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BatchNumber",
                table: "StockTransactions");

            migrationBuilder.DropColumn(
                name: "ExpiryDate",
                table: "StockTransactions");
        }
    }
}
