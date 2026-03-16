using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace tracking_service.Migrations
{
    /// <inheritdoc />
    public partial class v1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "user_sessions",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    started_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("user_sessions_pkey", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "tracking_events",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    session_id = table.Column<Guid>(type: "uuid", nullable: true),
                    event_type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    product_id = table.Column<Guid>(type: "uuid", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("tracking_events_pkey", x => x.id);
                    table.ForeignKey(
                        name: "tracking_events_session_id_fkey",
                        column: x => x.session_id,
                        principalTable: "user_sessions",
                        principalColumn: "id");
                });

            migrationBuilder.CreateIndex(
                name: "idx_tracking_event",
                table: "tracking_events",
                column: "event_type");

            migrationBuilder.CreateIndex(
                name: "idx_tracking_product",
                table: "tracking_events",
                column: "product_id");

            migrationBuilder.CreateIndex(
                name: "idx_tracking_user",
                table: "tracking_events",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_tracking_events_session_id",
                table: "tracking_events",
                column: "session_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tracking_events");

            migrationBuilder.DropTable(
                name: "user_sessions");
        }
    }
}
