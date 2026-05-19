using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BioInfoProject.Server.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Datasets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FileName = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    OriginalFileName = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    UploadedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    RowCount = table.Column<int>(type: "INTEGER", nullable: false),
                    CountryCount = table.Column<int>(type: "INTEGER", nullable: false),
                    RegionCount = table.Column<int>(type: "INTEGER", nullable: false),
                    MinDate = table.Column<DateOnly>(type: "TEXT", nullable: true),
                    MaxDate = table.Column<DateOnly>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Datasets", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EpidemiologicalRecords",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DatasetId = table.Column<int>(type: "INTEGER", nullable: false),
                    DateReported = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    CountryCode = table.Column<string>(type: "TEXT", maxLength: 10, nullable: false),
                    Country = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                    WhoRegion = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    NewCases = table.Column<int>(type: "INTEGER", nullable: false),
                    CumulativeCases = table.Column<int>(type: "INTEGER", nullable: false),
                    NewDeaths = table.Column<int>(type: "INTEGER", nullable: false),
                    CumulativeDeaths = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EpidemiologicalRecords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EpidemiologicalRecords_Datasets_DatasetId",
                        column: x => x.DatasetId,
                        principalTable: "Datasets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EpidemiologicalRecords_DatasetId",
                table: "EpidemiologicalRecords",
                column: "DatasetId");

            migrationBuilder.CreateIndex(
                name: "IX_EpidemiologicalRecords_DatasetId_Country",
                table: "EpidemiologicalRecords",
                columns: new[] { "DatasetId", "Country" });

            migrationBuilder.CreateIndex(
                name: "IX_EpidemiologicalRecords_DatasetId_DateReported",
                table: "EpidemiologicalRecords",
                columns: new[] { "DatasetId", "DateReported" });

            migrationBuilder.CreateIndex(
                name: "IX_EpidemiologicalRecords_DatasetId_WhoRegion",
                table: "EpidemiologicalRecords",
                columns: new[] { "DatasetId", "WhoRegion" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EpidemiologicalRecords");

            migrationBuilder.DropTable(
                name: "Datasets");
        }
    }
}
