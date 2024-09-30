using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EcommerceOrderManagement.Migrations.Migrations
{
    /// <inheritdoc />
    public partial class AddColumnHasRefund : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "HasRefund",
                table: "PixPayments",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "HasRefund",
                table: "CardPayments",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HasRefund",
                table: "PixPayments");

            migrationBuilder.DropColumn(
                name: "HasRefund",
                table: "CardPayments");
        }
    }
}
