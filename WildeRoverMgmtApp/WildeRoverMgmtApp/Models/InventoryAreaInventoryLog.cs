using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WildeRoverMgmtApp.Models
{
    public class InventoryAreaInventoryLog
    {
        [Key]
        public int InventoryAreaInventoryLogId { get; set; }

        public DateTime Date { get; set; }

        public int InventoryAreaid { get; set; }
        public InventoryArea InventoryArea { get; set; }
        
        public int InventorySummaryId { get; set; }
        public InventorySummary InventorySummary { get; set; }

        public List<ItemCount> Inventory { get; set; }
        
    }
}
