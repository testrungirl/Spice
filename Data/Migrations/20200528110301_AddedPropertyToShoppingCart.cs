using Microsoft.EntityFrameworkCore.Migrations;

namespace Spice.Data.Migrations
{
    public partial class AddedPropertyToShoppingCart : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MenuItemId1",
                table: "ShoppingCart",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ShoppingCart_MenuItemId1",
                table: "ShoppingCart",
                column: "MenuItemId1");

            migrationBuilder.AddForeignKey(
                name: "FK_ShoppingCart_MenuItem_MenuItemId1",
                table: "ShoppingCart",
                column: "MenuItemId1",
                principalTable: "MenuItem",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShoppingCart_MenuItem_MenuItemId1",
                table: "ShoppingCart");

            migrationBuilder.DropIndex(
                name: "IX_ShoppingCart_MenuItemId1",
                table: "ShoppingCart");

            migrationBuilder.DropColumn(
                name: "MenuItemId1",
                table: "ShoppingCart");
        }
    }
}
