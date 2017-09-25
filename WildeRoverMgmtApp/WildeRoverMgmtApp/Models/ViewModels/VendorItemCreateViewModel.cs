using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace WildeRoverMgmtApp.Models
{
    public class VendorItemCreateEditViewModel
    {
        public int VendorItemId { get; set; }

        [Required]
        public string Name { get; set; }
        [Required]
        public int VendorId { get; set; }
        [Required]
        public int WildeRoverItemId { get; set; }

        [Required]
        public int PackSize { get; set; }

        [Required]
        [DisplayFormat(DataFormatString ="{0:C}")]
        public decimal Price { get; set; }
        
    }
}
