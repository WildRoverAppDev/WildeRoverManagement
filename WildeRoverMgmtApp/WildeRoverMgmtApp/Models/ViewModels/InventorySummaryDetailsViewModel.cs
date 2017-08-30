using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WildeRoverMgmtApp.Models
{
    public class InventorySummaryDetailsViewModel
    {
        public InventorySummaryDetailsViewModel()
        {
            Inventory = new SortedDictionary<WildeRoverItem, int>();
        }

        public SortedDictionary<WildeRoverItem, int> Inventory { get; set; }
        public InventorySummary Summary { get; set; }
    }
}
