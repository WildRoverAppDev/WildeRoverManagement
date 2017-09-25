using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WildeRoverMgmtApp.Models
{
    public class HomeIndexViewModel
    {
        public HomeIndexViewModel()
        {
            InventoryList = new List<InventorySummary>();
            OrderList = new List<OrderSummary>();
        }

        public List<InventorySummary> InventoryList { get; set; }
        public List<OrderSummary> OrderList { get; set; }
    }
}
