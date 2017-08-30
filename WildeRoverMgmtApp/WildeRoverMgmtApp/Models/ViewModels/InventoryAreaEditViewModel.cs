using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace WildeRoverMgmtApp.Models
{
    public class InventoryAreaEditViewModel
    {
        public int InventoryAreaId { get; set; }
        public InventoryArea InventoryArea { get; set; }
        
        public List<SelectListItem> ItemList { get; set; }
        public List<InventoryAreaEditEntry> SlotList { get; set; }
    }

    public class InventoryAreaEditEntry
    {
        public int InventoryAreaId { get; set; }
        public int WildeRoverItemId { get; set; }
        public int InventorySlotId { get; set; }
        public InventorySlot Slot { get; set; }
    }

    
}
