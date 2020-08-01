using Microsoft.EntityFrameworkCore.Migrations;

namespace Spice.Migrations
{
    public partial class addedMenuItemIdtoOrderDetails : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderDetails_MenuItem_MenuItemId",
                table: "OrderDetails");

            migrationBuilder.AlterColumn<int>(
                name: "MenuItemId",
                table: "OrderDetails",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDetails_MenuItem_MenuItemId",
                table: "OrderDetails",
                column: "MenuItemId",
                principalTable: "MenuItem",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderDetails_MenuItem_MenuItemId",
                table: "OrderDetails");

            migrationBuilder.AlterColumn<int>(
                name: "MenuItemId",
                table: "OrderDetails",
                type: "int",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDetails_MenuItem_MenuItemId",
                table: "OrderDetails",
                column: "MenuItemId",
                principalTable: "MenuItem",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
