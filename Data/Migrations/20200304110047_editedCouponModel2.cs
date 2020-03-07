using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Spice.Data.Migrations
{
    public partial class editedCouponModel2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Pcture",
                table: "Coupon");

            migrationBuilder.AddColumn<byte[]>(
                name: "Picture",
                table: "Coupon",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Picture",
                table: "Coupon");

            migrationBuilder.AddColumn<byte[]>(
                name: "Pcture",
                table: "Coupon",
                type: "varbinary(max)",
                nullable: true);
        }
    }
}
