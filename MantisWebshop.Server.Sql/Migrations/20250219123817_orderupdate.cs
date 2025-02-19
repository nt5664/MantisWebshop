using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MantisWebshop.Server.Sql.Migrations
{
    /// <inheritdoc />
    public partial class orderupdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_order_users_UserId",
                table: "order");

            migrationBuilder.AlterColumn<Guid>(
                name: "UserId",
                table: "order",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddForeignKey(
                name: "FK_order_users_UserId",
                table: "order",
                column: "UserId",
                principalTable: "users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_order_users_UserId",
                table: "order");

            migrationBuilder.AlterColumn<Guid>(
                name: "UserId",
                table: "order",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_order_users_UserId",
                table: "order",
                column: "UserId",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
