using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WildeRoverMgmtApp.Migrations
{
    public partial class AddWRItemIdToItemCount : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ItemCounts_WildeRoverItem_ItemWildeRoverItemId",
                table: "ItemCounts");

            migrationBuilder.DropIndex(
                name: "IX_ItemCounts_ItemWildeRoverItemId",
                table: "ItemCounts");

            migrationBuilder.DropColumn(
                name: "ItemWildeRoverItemId",
                table: "ItemCounts");

            migrationBuilder.AddColumn<int>(
                name: "WildeRoverItemId",
                table: "ItemCounts",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ItemCounts_WildeRoverItemId",
                table: "ItemCounts",
                column: "WildeRoverItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_ItemCounts_WildeRoverItem_WildeRoverItemId",
                table: "ItemCounts",
                column: "WildeRoverItemId",
                principalTable: "WildeRoverItem",
                principalColumn: "WildeRoverItemId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ItemCounts_WildeRoverItem_WildeRoverItemId",
                table: "ItemCounts");

            migrationBuilder.DropIndex(
                name: "IX_ItemCounts_WildeRoverItemId",
                table: "ItemCounts");

            migrationBuilder.DropColumn(
                name: "WildeRoverItemId",
                table: "ItemCounts");

            migrationBuilder.AddColumn<int>(
                name: "ItemWildeRoverItemId",
                table: "ItemCounts",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ItemCounts_ItemWildeRoverItemId",
                table: "ItemCounts",
                column: "ItemWildeRoverItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_ItemCounts_WildeRoverItem_ItemWildeRoverItemId",
                table: "ItemCounts",
                column: "ItemWildeRoverItemId",
                principalTable: "WildeRoverItem",
                principalColumn: "WildeRoverItemId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
