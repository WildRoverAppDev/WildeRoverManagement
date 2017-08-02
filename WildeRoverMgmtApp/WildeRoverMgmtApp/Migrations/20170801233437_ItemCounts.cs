using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WildeRoverMgmtApp.Migrations
{
    public partial class ItemCounts : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ItemCount_InventoryLog_InventorySummaryId",
                table: "ItemCount");

            migrationBuilder.DropForeignKey(
                name: "FK_ItemCount_WildeRoverItem_ItemWildeRoverItemId",
                table: "ItemCount");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ItemCount",
                table: "ItemCount");

            migrationBuilder.RenameTable(
                name: "ItemCount",
                newName: "ItemCounts");

            migrationBuilder.RenameIndex(
                name: "IX_ItemCount_ItemWildeRoverItemId",
                table: "ItemCounts",
                newName: "IX_ItemCounts_ItemWildeRoverItemId");

            migrationBuilder.RenameIndex(
                name: "IX_ItemCount_InventorySummaryId",
                table: "ItemCounts",
                newName: "IX_ItemCounts_InventorySummaryId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ItemCounts",
                table: "ItemCounts",
                column: "ItemCountId");

            migrationBuilder.AddForeignKey(
                name: "FK_ItemCounts_InventoryLog_InventorySummaryId",
                table: "ItemCounts",
                column: "InventorySummaryId",
                principalTable: "InventoryLog",
                principalColumn: "InventorySummaryId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ItemCounts_WildeRoverItem_ItemWildeRoverItemId",
                table: "ItemCounts",
                column: "ItemWildeRoverItemId",
                principalTable: "WildeRoverItem",
                principalColumn: "WildeRoverItemId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ItemCounts_InventoryLog_InventorySummaryId",
                table: "ItemCounts");

            migrationBuilder.DropForeignKey(
                name: "FK_ItemCounts_WildeRoverItem_ItemWildeRoverItemId",
                table: "ItemCounts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ItemCounts",
                table: "ItemCounts");

            migrationBuilder.RenameTable(
                name: "ItemCounts",
                newName: "ItemCount");

            migrationBuilder.RenameIndex(
                name: "IX_ItemCounts_ItemWildeRoverItemId",
                table: "ItemCount",
                newName: "IX_ItemCount_ItemWildeRoverItemId");

            migrationBuilder.RenameIndex(
                name: "IX_ItemCounts_InventorySummaryId",
                table: "ItemCount",
                newName: "IX_ItemCount_InventorySummaryId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ItemCount",
                table: "ItemCount",
                column: "ItemCountId");

            migrationBuilder.AddForeignKey(
                name: "FK_ItemCount_InventoryLog_InventorySummaryId",
                table: "ItemCount",
                column: "InventorySummaryId",
                principalTable: "InventoryLog",
                principalColumn: "InventorySummaryId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ItemCount_WildeRoverItem_ItemWildeRoverItemId",
                table: "ItemCount",
                column: "ItemWildeRoverItemId",
                principalTable: "WildeRoverItem",
                principalColumn: "WildeRoverItemId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
