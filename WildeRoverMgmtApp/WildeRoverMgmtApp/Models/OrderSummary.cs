using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace WildeRoverMgmtApp.Models
{
    public class OrderSummary
    {
        public OrderSummary()
        {
            OrderList = new List<ItemCount>();
        }

        [Key]
        public int OrderSummaryId { get; set; }
        public DateTime Date { get; set; }
        public bool Completed { get; set; }

        [DisplayFormat(DataFormatString = "{0:C}")]
        public decimal Total { get; set; }

        [Display(Name = "Last Edited By")]
        public string LastEdited { get; set; }

        public virtual List<ItemCount> OrderList { get; set; }

        public decimal CalculateTotal()
        {
            decimal total = 0m;

            foreach(var ic in OrderList)
            {
                total += ic.Item.DefaultVendorItem.Price * ic.Count;
            }

            return total;
        }
    }
}
