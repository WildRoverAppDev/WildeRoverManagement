using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WildeRoverMgmtApp.Models
{
    public class OrderViewModel
    {
        public OrderViewModel()
        {
            OrderList = new List<ItemCount>();
        }

        public int OrderId { get; set; }

        public List<ItemCount> OrderList { get; set; }

    }
}
