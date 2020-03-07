using Microsoft.EntityFrameworkCore.Migrations;

namespace Spice.Data.Migrations
{
    public partial class editedCouponModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Dscount",
                table: "Coupon");

            migrationBuilder.AddColumn<double>(
                name: "Discount",
                table: "Coupon",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Discount",
                table: "Coupon");

            migrationBuilder.AddColumn<double>(
                name: "Dscount",
                table: "Coupon",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }
    }
}
