using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MantisWebshop.Server.Sql.Migrations
{
    /// <inheritdoc />
    public partial class morefixes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Quantity",
                table: "productSnapshots",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "productSnapshots");
        }
    }
}
