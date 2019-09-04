using Microsoft.EntityFrameworkCore.Migrations;

namespace Persistence.Migrations
{
    public partial class AddValues : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Values",
                columns: new[] { "ID", "Name" },
                values: new object[] { 1, "Name 1" });

            migrationBuilder.InsertData(
                table: "Values",
                columns: new[] { "ID", "Name" },
                values: new object[] { 2, "Name 2" });

            migrationBuilder.InsertData(
                table: "Values",
                columns: new[] { "ID", "Name" },
                values: new object[] { 3, "Name 3" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Values",
                keyColumn: "ID",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Values",
                keyColumn: "ID",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Values",
                keyColumn: "ID",
                keyValue: 3);
        }
    }
}
