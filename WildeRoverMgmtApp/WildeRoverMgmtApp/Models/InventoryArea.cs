using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WildeRoverMgmtApp.Models
{
    public class InventoryArea
    {
        public InventoryArea()
        {
            ItemSlots = new List<InventorySlot>();
        }

        [Key]
        public int InventoryAreaId { get; set; }
        public string Name { get; set; }

        public WildeRoverItem.House House { get; set; }

        public virtual List<InventorySlot> ItemSlots {get; set;}

    }
}
