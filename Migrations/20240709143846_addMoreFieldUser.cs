using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProductManagement.Migrations
{
    /// <inheritdoc />
    public partial class addMoreFieldUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UserCreateId",
                table: "Products",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UserUpdateId",
                table: "Products",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Products_UserCreateId",
                table: "Products",
                column: "UserCreateId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_UserUpdateId",
                table: "Products",
                column: "UserUpdateId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Products_UserCreateId",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_UserUpdateId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "UserCreateId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "UserUpdateId",
                table: "Products");
        }
    }
}
