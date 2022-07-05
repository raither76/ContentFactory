using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ContentFactory.Data.Migrations
{
    public partial class _30052201 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FaceImg",
                table: "OrderFiles");

            migrationBuilder.AddColumn<int>(
                name: "Quontity",
                table: "Orders",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Quontity",
                table: "Orders");

            migrationBuilder.AddColumn<bool>(
                name: "FaceImg",
                table: "OrderFiles",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
