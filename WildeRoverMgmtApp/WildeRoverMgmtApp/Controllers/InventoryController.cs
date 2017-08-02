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
        private InventorySummary _summary;

        public InventoryController(WildeRoverMgmtAppContext context)
        {
            _context = context;

            //Get latest inventory summary if it exists
            var current = (from i in _context.InventoryLog.Include("Inventory.Item")
                           where i.Date.Date == DateTime.Now.Date
                           select i).SingleOrDefault();

            //No inventory log created for the day
            if (current == null)
            {
                _summary = new InventorySummary();
                _summary.Date = DateTime.Now;

                _context.InventoryLog.Add(_summary);
                _context.SaveChanges();

                _summary = (from i in _context.InventoryLog.Include("Inventory.Item")
                            where i.Date.Date == DateTime.Now.Date
                            select i).SingleOrDefault();

                //populate summary
                var items = (from i in _context.WildeRoverItem
                             select i).ToList();

                foreach(var i in items)
                {
                    var temp = new ItemCount
                    {
                        Item = i,
                        WildeRoverItemId = i.WildeRoverItemId,
                        Count = 0
                    };

                    temp.InventorySummary = _summary;
                    temp.InventorySummaryId = _summary.InventorySummaryId;

                    _context.ItemCounts.Add(temp);
                    _summary.Inventory.Add(temp);
                }

                _context.InventoryLog.Update(_summary);
                _context.SaveChanges();
            }

            else  //Log exists
            {
                _summary = current;
            }

            //_summary.Inventory.ToAsyncEnumerable<ItemCount>();
        }

        public IActionResult Index()
        {
            return View();
        }

        //GET
        public async Task<IActionResult> FrontHouseInventory()
        {

            //var items = from i in _summary.Inventory.ToAsyncEnumerable<ItemCount>()
            //            where i.Item.ItemHouse.HasFlag(WildeRoverItem.House.front)
            //            orderby i.Item.Type, i.Item.Name
            //            select i;

            var items = from i in _summary.Inventory.ToAsyncEnumerable()
                        where i.Item.ItemHouse.HasFlag(WildeRoverItem.House.front)
                        orderby i.Item.Type, i.Item.Name
                        select i;


            FrontHouseInventoryViewModel fhivm = new FrontHouseInventoryViewModel();
            //var temp = await items.ToListAsync();

            fhivm.Inventory.AddRange(await items.ToList());     

            return View(fhivm);
        }

        [HttpPost]
        public async Task<IActionResult> FrontHouseInventory(FrontHouseInventoryViewModel fhivm)
        {
            if (fhivm == null)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    foreach(var ic in fhivm.Inventory)
                    {
                        var temp = await (from i in _context.ItemCounts
                                    where i.ItemCountId == ic.ItemCountId
                                    select i).SingleOrDefaultAsync();

                        temp.Count = ic.Count;

                        _context.ItemCounts.Update(temp);
                    }                                 

                    await _context.SaveChangesAsync();
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