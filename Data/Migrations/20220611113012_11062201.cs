using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ContentFactory.Data.Migrations
{
    public partial class _11062201 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "PeerId",
                table: "AspNetUsers",
                type: "bigint",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PeerId",
                table: "AspNetUsers");
        }
    }
}
