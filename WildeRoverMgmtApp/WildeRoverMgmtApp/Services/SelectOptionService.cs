using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using WildeRoverMgmtApp.Models;

namespace WildeRoverMgmtApp
{
    //Service to provides select options for View drop down lists
    public class SelectOptionService
    {
        private readonly WildeRoverMgmtAppContext _context;

        public SelectOptionService(WildeRoverMgmtAppContext context)
        {
            _context = context;
        }

        public async Task<List<SelectListItem>> ListWildeRoverItems()
        {
            var items = await (from item in _context.WildeRoverItem
                               orderby item.Name
                               select item).ToListAsync();

            var itemList = new List<SelectListItem>();

            itemList.Add(new SelectListItem { Text = "Please select a house item.", Value = "0" });
            foreach(var item in items)
            {
                itemList.Add(new SelectListItem { Text = item.Name, Value = item.WildeRoverItemId.ToString() });
            }

            return itemList;
        }

        public async Task<List<SelectListItem>> ListVendors()
        {
            var vendors = await (from vendor in _context.Vendor
                                 orderby vendor.Name
                                 select vendor).ToListAsync();

            var vendorList = new List<SelectListItem>();

            vendorList.Add(new SelectListItem { Text = "Please select a vendor.", Value = "0" });
            foreach(var vendor in vendors)
            {
                vendorList.Add(new SelectListItem { Text = vendor.Name, Value = vendor.VendorId.ToString() });
            }

            return vendorList;
        }

    }
}
