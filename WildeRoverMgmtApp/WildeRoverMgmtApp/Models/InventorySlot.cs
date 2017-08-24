using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WildeRoverMgmtApp.Models
{
    public class InventorySlot
    {
        [Key]
        public int InventorySlotId { get; set; }

        public int Slot { get; set; }

        public int InventoryAreaId { get; set; }
        public InventoryArea InventoryArea { get; set; }

        public int WildeRoverItemId { get; set; }
        public WildeRoverItem WildeRoverItem { get; set; }
    }
}
