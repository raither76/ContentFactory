using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ContentFactory.Data.Migrations
{
    public partial class _26052201 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Price",
                table: "Catalogs",
                newName: "Price9");

            migrationBuilder.AlterColumn<string>(
                name: "Bust",
                table: "Models",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<double>(
                name: "Price12",
                table: "Catalogs",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Price15",
                table: "Catalogs",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Price3",
                table: "Catalogs",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Price6",
                table: "Catalogs",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Price12",
                table: "Catalogs");

            migrationBuilder.DropColumn(
                name: "Price15",
                table: "Catalogs");

            migrationBuilder.DropColumn(
                name: "Price3",
                table: "Catalogs");

            migrationBuilder.DropColumn(
                name: "Price6",
                table: "Catalogs");

            migrationBuilder.RenameColumn(
                name: "Price9",
                table: "Catalogs",
                newName: "Price");

            migrationBuilder.AlterColumn<string>(
                name: "Bust",
                table: "Models",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }
    }
}
