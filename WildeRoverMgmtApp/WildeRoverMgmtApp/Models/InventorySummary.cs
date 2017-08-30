using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace WildeRoverMgmtApp.Models
{
    public class InventorySummary
    {
        public InventorySummary()
        {
            //Inventory = new List<ItemCount>();
        }

        [Key]
        public int InventorySummaryId { get; set; }
        public DateTime Date { get; set; }
        public bool Submitted { get; set; }
        //public virtual List<ItemCount> Inventory { get; set; }
        public virtual List<InventoryAreaInventoryLog> InventoryAreaLogs { get; set; }

    }
}
