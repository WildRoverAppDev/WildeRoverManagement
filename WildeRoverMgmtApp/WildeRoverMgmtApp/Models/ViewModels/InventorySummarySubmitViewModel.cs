using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WildeRoverMgmtApp.Models
{
    public class InventorySummarySubmitViewModel
    {
        public InventorySummarySubmitViewModel()
        {
            SubItems = new SortedDictionary<string, List<ItemCount>>();
        }

        public int InventorySummaryId { get; set; }
        public InventorySummary Summary { get; set; }
                
        public SortedDictionary<string, List<ItemCount>> SubItems { get; set; }
    }
}
