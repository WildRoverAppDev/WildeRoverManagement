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
    public class WildeRoverMgmtAppContext : IdentityDbContext<User, IdentityRole, string>
    {
        public WildeRoverMgmtAppContext (DbContextOptions<WildeRoverMgmtAppContext> options)
            : base(options)
        {

        }

        public DbSet<WildeRoverMgmtApp.Models.WildeRoverItem> WildeRoverItem { get; set; }

        public DbSet<WildeRoverMgmtApp.Models.VendorItem> VendorItem { get; set; }

        public DbSet<WildeRoverMgmtApp.Models.Vendor> Vendor { get; set; }

        public DbSet<WildeRoverMgmtApp.Models.ItemCount> ItemCounts { get; set; }

        public DbSet<WildeRoverMgmtApp.Models.InventorySummary> InventoryLog { get; set; }


        public DbSet<WildeRoverMgmtApp.Models.OrderSummary> OrderLog { get; set; }


    }
}
