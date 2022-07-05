using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ContentFactory.Data.Migrations
{
    public partial class _01062204 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LocId",
                table: "OrderItems",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "StyleId",
                table: "OrderItems",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "VideoId",
                table: "OrderItems",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "photoNumber",
                table: "OrderItems",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LocId",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "StyleId",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "VideoId",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "photoNumber",
                table: "OrderItems");
        }
    }
}
