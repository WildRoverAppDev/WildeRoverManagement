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
                           orderby i.Date descending
                           select i).FirstOrDefault();

            //No inventory log created for the day
            if (current == null || current.Submitted)
            {
                _summary = new InventorySummary();
                _summary.Date = DateTime.Now;
                _summary.Submitted = false;

                _context.InventoryLog.Add(_summary);
                _context.SaveChanges();

                _summary = (from i in _context.InventoryLog.Include("Inventory.Item")
                            orderby i.Date descending
                            select i).FirstOrDefault();

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
                    temp.OrderSummary = null;

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
        public async Task<IActionResult> FrontHouseInventory(bool? loadFromContext)
        {
            var items = from i in _summary.Inventory.ToAsyncEnumerable()
                        where i.Item.ItemHouse.HasFlag(WildeRoverItem.House.front)
                        orderby i.Item.Type, i.Item.Name
                        select i;

            FrontHouseInventoryViewModel fhivm = new FrontHouseInventoryViewModel();
            fhivm.SummaryId = _summary.InventorySummaryId;

            if (loadFromContext == null || loadFromContext == true)
            {
                //Load fhivm with data from items

                foreach(var itemCount in await items.ToList())
                {
                    fhivm.Inventory.Add
                    (
                        new ItemCount
                        {
                            ItemCountId = itemCount.ItemCountId,
                            Item = itemCount.Item,
                            Count = itemCount.Count
                        }
                    );
                }
            }
            else
            {
                foreach(var itemCount in await items.ToList())
                {
                    fhivm.Inventory.Add
                    (
                        new ItemCount
                        {
                            ItemCountId = itemCount.ItemCountId,
                            Item = itemCount.Item,
                            Count = 0
                        }
                    );
                }
            }

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

                return RedirectToAction("Index", "InventorySummary");
            }

            return View(fhivm);
        }

        //Change Inventory View Model counts to 0
        public async Task<IActionResult> Reset()
        {
            return RedirectToAction("FrontHouseInventory", new { loadFromContext = false });
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