using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EcommerceOrderManagement.Migrations.Migrations
{
    /// <inheritdoc />
    public partial class AddColumnCardPaymentInstallments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Installments",
                table: "CardPayments",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Installments",
                table: "CardPayments");
        }
    }
}
