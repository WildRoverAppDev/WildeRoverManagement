using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WildeRoverMgmtApp.Models
{
    public class FrontHouseInventoryViewModel
    {
        //public Dictionary<WildeRoverItem, int> Inventory { get; set; }
        //public List<WildeRoverItem> Inventory { get; set; }
        //public List<WildeRoverItem> Inventory { get; set; }
        //public List<int> ItemCounts { get; set; }

        public FrontHouseInventoryViewModel()
        {
            Inventory = new List<ItemCount>();
        }


        public List<ItemCount> Inventory { get; set; }
    }
}
