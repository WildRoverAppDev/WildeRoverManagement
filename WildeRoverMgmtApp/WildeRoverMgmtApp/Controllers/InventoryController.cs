using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WildeRoverMgmtApp.Models;

namespace WildeRoverMgmtApp.Controllers
{
    public class InventoryController : Controller
    {
        private readonly WildeRoverMgmtAppContext _context;

        public InventoryController(WildeRoverMgmtAppContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        //GET
        //public async Task<IActionResult> FrontHouseInventory()
        //{
        //    var items = from i in _context.WildeRoverItem
        //                where i.ItemHouse == WildeRoverItem.House.front
        //                orderby i.Type, i.Name
        //                select i;

        //    var itemList = await items.ToListAsync();

        //    FrontHouseInventoryViewModel fhivm = new FrontHouseInventoryViewModel();
        //    fhivm.Inventory = new Dictionary<WildeRoverItem, int>();

        //    foreach (WildeRoverItem item in items)
        //    {
        //        fhivm.Inventory.Add(item, 0);
        //    }

        //    return View(fhivm);
        //}

        //GET
        public async Task<IActionResult> FrontHouseInventory()
        {
            var items = from i in _context.WildeRoverItem
                        where i.ItemHouse == WildeRoverItem.House.front
                        orderby i.Type, i.Name
                        select i;

            //FrontHouseInventoryViewModel fhivm = new FrontHouseInventoryViewModel();
            //fhivm.Inventory = await items.ToListAsync();

            return View(await items.ToListAsync());
        }

        [HttpPost]
        public async Task<IActionResult> FrontHouseInventory(List<WildeRoverItem> fhivm)
        {
            if (fhivm == null)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    foreach (var item in fhivm)
                    {
                        _context.Update(item);
                        await _context.SaveChangesAsync();
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    throw;
                }

                return RedirectToAction("Index");
            }

            return View(fhivm);
        }

        //GET
        public async Task<IActionResult> BackHouseInventory()
        {
            var items = from i in _context.WildeRoverItem
                        where i.ItemHouse == WildeRoverItem.House.back
                        orderby i.Type, i.Name
                        select i;

            //FrontHouseInventoryViewModel fhivm = new FrontHouseInventoryViewModel();
            //fhivm.Inventory = await items.ToListAsync();

            return View(await items.ToListAsync());
        }

        [HttpPost]
        public async Task<IActionResult> BackHouseInventory(List<WildeRoverItem> bhivm)
        {
            if (bhivm == null)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    foreach (var item in bhivm)
                    {
                        _context.Update(item);
                        await _context.SaveChangesAsync();
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    throw;
                }

                return RedirectToAction("Index");
            }

            return View(bhivm);
        }
    }
}