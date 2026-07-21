using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace food_service.ProductService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddProductDailyInventory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "product_daily_inventories",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuidv7()"),
                    product_id = table.Column<Guid>(type: "uuid", nullable: false),
                    inventory_date = table.Column<DateOnly>(type: "date", nullable: false),
                    initial_quantity = table.Column<int>(type: "integer", nullable: false, defaultValue: 100),
                    remaining_quantity = table.Column<int>(type: "integer", nullable: false, defaultValue: 100),
                    sold_quantity = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    is_available = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("product_daily_inventories_pkey", x => x.id);
                    table.CheckConstraint("ck_daily_inventory_initial_quantity", "initial_quantity >= 0");
                    table.CheckConstraint("ck_daily_inventory_quantity_balance", "remaining_quantity + sold_quantity = initial_quantity");
                    table.CheckConstraint("ck_daily_inventory_remaining_quantity", "remaining_quantity >= 0");
                    table.CheckConstraint("ck_daily_inventory_sold_quantity", "sold_quantity >= 0");
                    table.ForeignKey(
                        name: "fk_daily_inventory_product",
                        column: x => x.product_id,
                        principalTable: "products",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "uq_daily_inventory_product_date",
                table: "product_daily_inventories",
                columns: new[] { "product_id", "inventory_date" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "product_daily_inventories");
        }
    }
}
