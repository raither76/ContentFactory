using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ContentFactory.Data.Migrations
{
    public partial class _01062203 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StyleDescription",
                table: "Orders");

            migrationBuilder.RenameColumn(
                name: "Desription",
                table: "OrderItems",
                newName: "StyleDescription");

            migrationBuilder.AddColumn<string>(
                name: "FooterDesription",
                table: "OrderItems",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FooterDesription",
                table: "OrderItems");

            migrationBuilder.RenameColumn(
                name: "StyleDescription",
                table: "OrderItems",
                newName: "Desription");

            migrationBuilder.AddColumn<string>(
                name: "StyleDescription",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
