using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace auth_services.Migrations
{
    /// <inheritdoc />
    public partial class v2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OutBoxMessageTable",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TypeMessage = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PayLoad = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsProccessed = table.Column<bool>(type: "bit", nullable: false),
                    CreateAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OutBoxMessageTable", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OutBoxMessageTable");
        }
    }
}
