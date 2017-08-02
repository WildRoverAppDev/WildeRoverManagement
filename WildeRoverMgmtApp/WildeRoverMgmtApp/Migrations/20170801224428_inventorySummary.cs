using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace WildeRoverMgmtApp.Migrations
{
    public partial class inventorySummary : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "InventoryLog",
                columns: table => new
                {
                    InventorySummaryId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Date = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryLog", x => x.InventorySummaryId);
                });

            migrationBuilder.CreateTable(
                name: "ItemCount",
                columns: table => new
                {
                    ItemCountId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Count = table.Column<int>(nullable: false),
                    InventorySummaryId = table.Column<int>(nullable: true),
                    ItemWildeRoverItemId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemCount", x => x.ItemCountId);
                    table.ForeignKey(
                        name: "FK_ItemCount_InventoryLog_InventorySummaryId",
                        column: x => x.InventorySummaryId,
                        principalTable: "InventoryLog",
                        principalColumn: "InventorySummaryId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ItemCount_WildeRoverItem_ItemWildeRoverItemId",
                        column: x => x.ItemWildeRoverItemId,
                        principalTable: "WildeRoverItem",
                        principalColumn: "WildeRoverItemId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ItemCount_InventorySummaryId",
                table: "ItemCount",
                column: "InventorySummaryId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemCount_ItemWildeRoverItemId",
                table: "ItemCount",
                column: "ItemWildeRoverItemId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ItemCount");

            migrationBuilder.DropTable(
                name: "InventoryLog");
        }
    }
}
