using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace order_service.Migrations
{
    /// <inheritdoc />
    public partial class v11 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "OrderStatusPayment",
                table: "Orders",
                newName: "OrderStatus");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "OrderStatus",
                table: "Orders",
                newName: "OrderStatusPayment");
        }
    }
}
