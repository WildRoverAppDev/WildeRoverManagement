using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using WildeRoverMgmtApp.Models;
using Microsoft.Extensions.Configuration;

namespace WildeRoverMgmtApp.Models
{
    public class WildeRoverMgmtAppContext : IdentityDbContext<User, Role, string>
    {
        public WildeRoverMgmtAppContext(DbContextOptions<WildeRoverMgmtAppContext> options)
            : base(options)
        {

        }

        public DbSet<WildeRoverMgmtApp.Models.WildeRoverItem> WildeRoverItem { get; set; }

        public DbSet<WildeRoverMgmtApp.Models.VendorItem> VendorItem { get; set; }

        public DbSet<WildeRoverMgmtApp.Models.Vendor> Vendor { get; set; }

        public DbSet<WildeRoverMgmtApp.Models.ItemCount> ItemCounts { get; set; }

        public DbSet<WildeRoverMgmtApp.Models.InventorySummary> InventoryLog { get; set; }

        public DbSet<WildeRoverMgmtApp.Models.OrderSummary> OrderLog { get; set; }

        public DbSet<WildeRoverMgmtApp.Models.InventoryArea> InventoryAreas { get; set; }

        public DbSet<WildeRoverMgmtApp.Models.InventorySlot> Slots { get; set; }

        public DbSet<WildeRoverMgmtApp.Models.InventoryAreaInventoryLog> InventoryAreaLogs { get; set; }

        //public DbSet<WildeRoverMgmtApp.Models.InventoryAreaWildeRoverItem> InventoryAreaWildeRoverItems { get; set; }

        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    base.OnModelCreating(modelBuilder);

        //    modelBuilder.Entity<InventoryAreaWildeRoverItem>()
        //            .HasKey(iw => new
        //            {
        //                iw.InventoryAreaId,
        //                iw.WildeRoverItemId
        //            });

        //    modelBuilder.Entity<InventoryAreaWildeRoverItem>()
        //        .HasOne(iw => iw.InventoryArea)
        //        .WithMany(ia => ia.ItemSlots)
        //        .HasForeignKey(iw => iw.InventoryAreaId);

        //    modelBuilder.Entity<InventoryAreaWildeRoverItem>()
        //                .HasOne(iw => iw.WildeRoverItem)
        //                .WithMany(w => w.InventoryAreaPlacements)
        //                .HasForeignKey(iw => iw.WildeRoverItemId);

        //}
        
    
    }

}
