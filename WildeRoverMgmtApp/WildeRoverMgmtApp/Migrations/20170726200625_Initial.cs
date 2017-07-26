using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace WildeRoverMgmtApp.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Vendor",
                columns: table => new
                {
                    VendorId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    EMail = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Phone = table.Column<string>(nullable: true),
                    PointOfContact = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vendor", x => x.VendorId);
                });

            migrationBuilder.CreateTable(
                name: "WildeRoverItem",
                columns: table => new
                {
                    WildeRoverItemId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Have = table.Column<int>(nullable: false),
                    InventoryCount = table.Column<int>(nullable: false),
                    ItemHouse = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    OrderCount = table.Column<int>(nullable: false),
                    Par = table.Column<int>(nullable: false),
                    Type = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WildeRoverItem", x => x.WildeRoverItemId);
                });

            migrationBuilder.CreateTable(
                name: "VendorItem",
                columns: table => new
                {
                    VendorItemId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    PackSize = table.Column<int>(nullable: false),
                    Price = table.Column<decimal>(nullable: false),
                    VendorId = table.Column<int>(nullable: false),
                    WildeRoverItemId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VendorItem", x => x.VendorItemId);
                    table.ForeignKey(
                        name: "FK_VendorItem_Vendor_VendorId",
                        column: x => x.VendorId,
                        principalTable: "Vendor",
                        principalColumn: "VendorId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VendorItem_WildeRoverItem_WildeRoverItemId",
                        column: x => x.WildeRoverItemId,
                        principalTable: "WildeRoverItem",
                        principalColumn: "WildeRoverItemId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_VendorItem_VendorId",
                table: "VendorItem",
                column: "VendorId");

            migrationBuilder.CreateIndex(
                name: "IX_VendorItem_WildeRoverItemId",
                table: "VendorItem",
                column: "WildeRoverItemId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VendorItem");

            migrationBuilder.DropTable(
                name: "Vendor");

            migrationBuilder.DropTable(
                name: "WildeRoverItem");
        }
    }
}
