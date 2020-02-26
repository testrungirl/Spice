using Microsoft.EntityFrameworkCore.Migrations;

namespace Spice.Data.Migrations
{
    public partial class RenamedmenuItemDbToMenuItemDb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_menuItem_Category_CategoryId",
                table: "menuItem");

            migrationBuilder.DropForeignKey(
                name: "FK_menuItem_SubCategory_SubCategoryId",
                table: "menuItem");

            migrationBuilder.DropPrimaryKey(
                name: "PK_menuItem",
                table: "menuItem");

            migrationBuilder.RenameTable(
                name: "menuItem",
                newName: "MenuItem");

            migrationBuilder.RenameIndex(
                name: "IX_menuItem_SubCategoryId",
                table: "MenuItem",
                newName: "IX_MenuItem_SubCategoryId");

            migrationBuilder.RenameIndex(
                name: "IX_menuItem_CategoryId",
                table: "MenuItem",
                newName: "IX_MenuItem_CategoryId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MenuItem",
                table: "MenuItem",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MenuItem_Category_CategoryId",
                table: "MenuItem",
                column: "CategoryId",
                principalTable: "Category",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_MenuItem_SubCategory_SubCategoryId",
                table: "MenuItem",
                column: "SubCategoryId",
                principalTable: "SubCategory",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MenuItem_Category_CategoryId",
                table: "MenuItem");

            migrationBuilder.DropForeignKey(
                name: "FK_MenuItem_SubCategory_SubCategoryId",
                table: "MenuItem");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MenuItem",
                table: "MenuItem");

            migrationBuilder.RenameTable(
                name: "MenuItem",
                newName: "menuItem");

            migrationBuilder.RenameIndex(
                name: "IX_MenuItem_SubCategoryId",
                table: "menuItem",
                newName: "IX_menuItem_SubCategoryId");

            migrationBuilder.RenameIndex(
                name: "IX_MenuItem_CategoryId",
                table: "menuItem",
                newName: "IX_menuItem_CategoryId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_menuItem",
                table: "menuItem",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_menuItem_Category_CategoryId",
                table: "menuItem",
                column: "CategoryId",
                principalTable: "Category",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_menuItem_SubCategory_SubCategoryId",
                table: "menuItem",
                column: "SubCategoryId",
                principalTable: "SubCategory",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }
    }
}
