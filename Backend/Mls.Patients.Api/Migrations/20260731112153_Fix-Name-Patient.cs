using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mls.Patients.Api.Migrations
{
    /// <inheritdoc />
    public partial class FixNamePatient : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Patients",
                keyColumn: "Id",
                keyValue: 4,
                column: "Nom",
                value: "TestEarlyOnset");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Patients",
                keyColumn: "Id",
                keyValue: 4,
                column: "Nom",
                value: "TestEarlyOnse");
        }
    }
}
