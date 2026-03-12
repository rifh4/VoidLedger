using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VoidLedger.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddPriceMovementMetadata : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "PreviousPrice",
                table: "Prices",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAtUtc",
                table: "Prices",
                type: "datetime2",
                nullable: true);

            migrationBuilder.Sql("""
                UPDATE [Prices]
                SET [UpdatedAtUtc] = SYSUTCDATETIME()
                WHERE [UpdatedAtUtc] IS NULL
                """);

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAtUtc",
                table: "Prices",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PreviousPrice",
                table: "Prices");

            migrationBuilder.DropColumn(
                name: "UpdatedAtUtc",
                table: "Prices");
        }
    }
}
