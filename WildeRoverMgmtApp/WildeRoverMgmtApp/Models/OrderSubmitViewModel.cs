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
            OrderItems = new SortedDictionary<string, List<ItemCount>>();
        }

        public int OrderSummaryId { get; set; }
        public OrderSummary Order { get; set; }
        public decimal Total { get; set; }

        public SortedDictionary<string, List<ItemCount>> OrderItems { get; set; }
    }
}
