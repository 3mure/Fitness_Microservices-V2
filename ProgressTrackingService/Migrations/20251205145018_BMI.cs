using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProgressTrackingService.Migrations.FitnessAppDb
{
    /// <inheritdoc />
    public partial class BMI : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Bmi",
                table: "WeightHistory",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Bmi",
                table: "WeightHistory");
        }
    }
}
