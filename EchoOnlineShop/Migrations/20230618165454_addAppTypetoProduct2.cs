using Microsoft.EntityFrameworkCore.Migrations;

namespace EchoOnlineShop.Migrations
{
    public partial class addAppTypetoProduct2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AppTypeID",
                table: "Product",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.CreateIndex(
                name: "IX_Product_AppTypeID",
                table: "Product",
                column: "AppTypeID");

            migrationBuilder.AddForeignKey(
                name: "FK_Product_AppType_AppTypeID",
                table: "Product",
                column: "AppTypeID",
                principalTable: "AppType",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Product_AppType_AppTypeID",
                table: "Product");

            migrationBuilder.DropIndex(
                name: "IX_Product_AppTypeID",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "AppTypeID",
                table: "Product");
        }
    }
}
