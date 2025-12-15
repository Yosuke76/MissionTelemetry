using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MissionTelemetry.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TelemetrySample",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TimeStamp = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Key = table.Column<string>(type: "TEXT", maxLength: 128, nullable: false),
                    Value = table.Column<double>(type: "REAL", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TelemetrySample", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TelemetrySample_Key_TimeStamp",
                table: "TelemetrySample",
                columns: new[] { "Key", "TimeStamp" });

            migrationBuilder.CreateIndex(
                name: "IX_TelemetrySample_TimeStamp",
                table: "TelemetrySample",
                column: "TimeStamp");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TelemetrySample");
        }
    }
}
