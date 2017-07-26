using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace WildeRoverMgmtApp.Models
{
    public partial class VendorItem
    {
        [Key]
        public int VendorItemId { get; set; }
        public string Name { get; set; }
        public int PackSize { get; set; }
        public decimal Price { get; set; }
        
        public int VendorId { get; set; }
        public virtual Vendor Vendor { get; set; }

        public int WildeRoverItemId { get; set; }
        public virtual WildeRoverItem WildeRoverItem { get; set; }

        [NotMapped]
        public decimal PPU { get { return Price / PackSize; } }
    }
}
