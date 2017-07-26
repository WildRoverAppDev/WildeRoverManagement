using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace WildeRoverMgmtApp.Models
{
    public partial class Vendor
    {
        [Key]
        public int VendorId { get; set; }
        public string Name { get; set; }        
        public string Phone { get; set; }
        public string EMail { get; set; }
        public string PointOfContact { get; set; }

        public virtual HashSet<VendorItem> Products { get; set; }        
    }
}
