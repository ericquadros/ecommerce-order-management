using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EcommerceOrderManagement.Migrations.Migrations
{
    /// <inheritdoc />
    public partial class ChangeColumnNameExpirationDate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ExpiryDate",
                table: "CardPayments",
                newName: "ExpirationDate");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ExpirationDate",
                table: "CardPayments",
                newName: "ExpiryDate");
        }
    }
}
