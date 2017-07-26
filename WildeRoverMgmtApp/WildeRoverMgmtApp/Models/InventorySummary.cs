using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WildeRoverMgmtApp.Models
{
    public class InventorySummary
    {
        public DateTime Date { get; set; }
        public SortedDictionary<WildeRoverItem, int> Inventory { get; set; }
    }
}
