using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HRManagementSystem.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemoveIsCurrentFromEmployeeSalary : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsCurrent",
                table: "EmployeeSalaries");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsCurrent",
                table: "EmployeeSalaries",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
