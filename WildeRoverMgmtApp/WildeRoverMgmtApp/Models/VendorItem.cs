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

        [Required]
        public string Name { get; set; }

        [Required]
        public int PackSize { get; set; }

        [DisplayFormat(DataFormatString = "{0:C}")]
        public decimal Price { get; set; }
        
        [Required]
        public int VendorId { get; set; }
        public virtual Vendor Vendor { get; set; }

        [Required]
        public int WildeRoverItemId { get; set; }
        public virtual WildeRoverItem WildeRoverItem { get; set; }

        [NotMapped]
        public decimal PPU { get { return Price / PackSize; } }
    }
}
