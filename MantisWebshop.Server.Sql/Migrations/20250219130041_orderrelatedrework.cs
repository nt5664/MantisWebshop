using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MantisWebshop.Server.Sql.Migrations
{
    /// <inheritdoc />
    public partial class orderrelatedrework : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_productSnapshots_products_ProductId",
                table: "productSnapshots");

            migrationBuilder.DropIndex(
                name: "IX_productSnapshots_ProductId",
                table: "productSnapshots");

            migrationBuilder.AlterColumn<Guid>(
                name: "ProductId",
                table: "productSnapshots",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "ProductId",
                table: "productSnapshots",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.CreateIndex(
                name: "IX_productSnapshots_ProductId",
                table: "productSnapshots",
                column: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_productSnapshots_products_ProductId",
                table: "productSnapshots",
                column: "ProductId",
                principalTable: "products",
                principalColumn: "Id");
        }
    }
}
