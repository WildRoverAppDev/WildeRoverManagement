using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WildeRoverMgmtApp.Models
{
    public class FrontHouseInventoryViewModel
    {
        public FrontHouseInventoryViewModel()
        {
            Inventory = new List<ItemCount>();
        }

        public InventoryAreaInventoryLog Log { get; set; }

        public List<ItemCount> Inventory { get; set; }
    }
}
