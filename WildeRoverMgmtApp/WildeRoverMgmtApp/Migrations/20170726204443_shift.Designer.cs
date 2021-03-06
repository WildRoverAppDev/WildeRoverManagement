﻿using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using WildeRoverMgmtApp.Models;

namespace WildeRoverMgmtApp.Migrations
{
    [DbContext(typeof(WildeRoverMgmtAppContext))]
    [Migration("20170726204443_shift")]
    partial class shift
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.2")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("WildeRoverMgmtApp.Models.Vendor", b =>
                {
                    b.Property<int>("VendorId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("EMail");

                    b.Property<string>("Name");

                    b.Property<string>("Phone");

                    b.Property<string>("PointOfContact");

                    b.HasKey("VendorId");

                    b.ToTable("Vendor");
                });

            modelBuilder.Entity("WildeRoverMgmtApp.Models.VendorItem", b =>
                {
                    b.Property<int>("VendorItemId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name");

                    b.Property<int>("PackSize");

                    b.Property<decimal>("Price");

                    b.Property<int>("VendorId");

                    b.Property<int>("WildeRoverItemId");

                    b.HasKey("VendorItemId");

                    b.HasIndex("VendorId");

                    b.HasIndex("WildeRoverItemId");

                    b.ToTable("VendorItem");
                });

            modelBuilder.Entity("WildeRoverMgmtApp.Models.WildeRoverItem", b =>
                {
                    b.Property<int>("WildeRoverItemId")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("Have");

                    b.Property<int>("InventoryCount");

                    b.Property<int>("ItemHouse");

                    b.Property<string>("Name");

                    b.Property<int>("OrderCount");

                    b.Property<int>("Par");

                    b.Property<string>("Type");

                    b.HasKey("WildeRoverItemId");

                    b.ToTable("WildeRoverItem");
                });

            modelBuilder.Entity("WildeRoverMgmtApp.Models.VendorItem", b =>
                {
                    b.HasOne("WildeRoverMgmtApp.Models.Vendor", "Vendor")
                        .WithMany("Products")
                        .HasForeignKey("VendorId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("WildeRoverMgmtApp.Models.WildeRoverItem", "WildeRoverItem")
                        .WithMany("VendorItems")
                        .HasForeignKey("WildeRoverItemId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
        }
    }
}
