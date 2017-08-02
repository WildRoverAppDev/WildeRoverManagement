using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WildeRoverMgmtApp.Models
{
    public class ItemCount
    {
        [Key]
        public int ItemCountId { get; set; }


        public int WildeRoverItemId { get; set; }
        [ForeignKey("WildeRoverItemId")]
        public WildeRoverItem Item { get; set; }

        public int Count { get; set; }

        public int InventorySummaryId { get; set; }
        public InventorySummary InventorySummary { get; set; }
    }
}
