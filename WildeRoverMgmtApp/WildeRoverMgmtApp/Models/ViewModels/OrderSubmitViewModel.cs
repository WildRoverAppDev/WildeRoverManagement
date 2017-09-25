using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WildeRoverMgmtApp.Models
{
    public class OrderSubmitViewModel
    {
        public OrderSubmitViewModel()
        {
            OrderItems = new SortedDictionary<Vendor, List<ItemCount>>();
        }

        public int OrderSummaryId { get; set; }
        public OrderSummary Order { get; set; }
        public decimal Total { get; set; }

        public SortedDictionary<Vendor, List<ItemCount>> OrderItems { get; set; }
    }
}
