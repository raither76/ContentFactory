using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ContentFactory.Data.Migrations
{
    public partial class _25052201 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CategoryId",
                table: "Models",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "Models");
        }
    }
}
