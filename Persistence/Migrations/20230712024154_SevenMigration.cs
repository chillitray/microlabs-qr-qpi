using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class SevenMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProductAnalyticsMetaDb",
                columns: table => new
                {
                    product_analytics_meta_db = table.Column<Guid>(type: "char(36)", nullable: false),
                    product_id = table.Column<Guid>(type: "char(36)", nullable: false),
                    qr_generated = table.Column<int>(type: "int", nullable: false),
                    qr_printed = table.Column<int>(type: "int", nullable: false),
                    qr_scanned = table.Column<int>(type: "int", nullable: false),
                    counterfeit_scans = table.Column<int>(type: "int", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductAnalyticsMetaDb", x => x.product_analytics_meta_db);
                })
                .Annotation("MySQL:Charset", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductAnalyticsMetaDb");
        }
    }
}
