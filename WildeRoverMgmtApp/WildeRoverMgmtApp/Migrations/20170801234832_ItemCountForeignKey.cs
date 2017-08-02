using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WildeRoverMgmtApp.Migrations
{
    public partial class ItemCountForeignKey : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ItemCounts_InventoryLog_InventorySummaryId",
                table: "ItemCounts");

            migrationBuilder.AlterColumn<int>(
                name: "InventorySummaryId",
                table: "ItemCounts",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ItemCounts_InventoryLog_InventorySummaryId",
                table: "ItemCounts",
                column: "InventorySummaryId",
                principalTable: "InventoryLog",
                principalColumn: "InventorySummaryId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ItemCounts_InventoryLog_InventorySummaryId",
                table: "ItemCounts");

            migrationBuilder.AlterColumn<int>(
                name: "InventorySummaryId",
                table: "ItemCounts",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddForeignKey(
                name: "FK_ItemCounts_InventoryLog_InventorySummaryId",
                table: "ItemCounts",
                column: "InventorySummaryId",
                principalTable: "InventoryLog",
                principalColumn: "InventorySummaryId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
