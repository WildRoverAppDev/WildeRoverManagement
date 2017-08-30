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
            var current = (from i in _context.InventoryLog.Include("InventoryAreaLogs.Inventory.Item")
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

                _summary = (from i in _context.InventoryLog.Include("InventoryAreaLogs.Inventory.Item")
                            orderby i.Date descending
                            select i).FirstOrDefault();

                //populate summary
                //var items = (from i in _context.WildeRoverItem
                //             select i).ToList();

                //foreach(var i in items)
                //{
                //    var temp = new ItemCount
                //    {
                //        Item = i,
                //        WildeRoverItemId = i.WildeRoverItemId,
                //        Count = 0
                //    };

                //    temp.InventorySummary = _summary;
                //    temp.InventorySummaryId = _summary.InventorySummaryId;
                //    temp.OrderSummary = null;

                //    _context.ItemCounts.Add(temp);
                //    _summary.Inventory.Add(temp);
                //}

                _context.InventoryLog.Update(_summary);
                

                CreateInventoryAreaLogs(_summary);

                _context.SaveChanges();
            }

            else  //Log exists
            {
                _summary = current;
            }

            //_summary.Inventory.ToAsyncEnumerable<ItemCount>();
        }

        private void CreateInventoryAreaLogs(InventorySummary summary)
        {
            var areas = from a in _context.InventoryAreas.Include("ItemSlots.WildeRoverItem")
                        select a;

            foreach(var area in areas.ToList())
            {
                //Create area inventory log
                InventoryAreaInventoryLog log = new InventoryAreaInventoryLog();
                log.InventorySummary = summary;
                log.InventorySummaryId = summary.InventorySummaryId;
                log.InventoryArea = area;
                log.InventoryAreaid = area.InventoryAreaId;
                log.Date = DateTime.Now;

                //Add to context and retrieve Id

                _context.InventoryAreaLogs.Add(log);
                _context.SaveChanges();

                log = _context.InventoryAreaLogs.Last();

                //populate the Inventory
                var items = from i in area.ItemSlots
                            orderby i.Slot ascending
                            select i.WildeRoverItem;

                foreach(var item in items)
                {
                    ItemCount temp = new ItemCount();
                    temp.InventoryAreaInventoryLog = log;
                    temp.InventoryAreaInventoryLogId = log.InventoryAreaInventoryLogId;
                    temp.Item = item;
                    temp.WildeRoverItemId = item.WildeRoverItemId;

                    _context.ItemCounts.Add(temp);

                    //Add ItemCount to area log
                    log.Inventory.Add(temp);
                }

                //Update log
                _context.InventoryAreaLogs.Update(log);

                //Add area inventory log to summary
                summary.InventoryAreaLogs.Add(log);
            }

            //Update summary
            _context.InventoryLog.Update(summary);

            //_context.SaveChanges();
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> FrontHouseIndex()
        {
            var areas = await (from a in _context.InventoryAreas
                         select a).ToListAsync();

            return View(areas);
        }
                
        [HttpGet]
        public async Task<IActionResult> FrontHouseInventory(int id, bool? loadFromContext)
        {
            //var items = from i in _summary.Inventory.ToAsyncEnumerable()
            //            where i.Item.ItemHouse.HasFlag(WildeRoverItem.House.front)
            //            orderby i.Item.Type, i.Item.Name
            //            select i;

            //retrieve area log (Get the most recent created)

            //var areaLog = await (from al in _context.InventoryAreaLogs.Include("Inventory.Item")
            //                     where al.InventoryAreaid == id
            //                     select al).LastOrDefaultAsync();

            var areaLog = (from al in _summary.InventoryAreaLogs
                           where al.InventoryAreaid == id
                           select al).SingleOrDefault();

            if (areaLog == null) return NotFound();
            
            //Create View Model
            FrontHouseInventoryViewModel fhivm = new FrontHouseInventoryViewModel();
            //fhivm.SummaryId = _summary.InventorySummaryId;
            //fhivm.InventoryAreaLogId = areaLog.InventoryAreaInventoryLogId;
            fhivm.Log = areaLog;

            foreach(var ic in areaLog.Inventory)
            {
                ItemCount temp = new ItemCount();
                temp.ItemCountId = ic.ItemCountId;
                temp.Item = ic.Item;

                if (loadFromContext == null || loadFromContext == true)
                {
                    temp.Count = ic.Count;
                }
                else
                {
                    temp.Count = 0;
                }

                fhivm.Inventory.Add(temp);
            }

            return View(fhivm);

            //if (loadFromContext == null || loadFromContext == true)
            //{
            //    //Load fhivm with data from items

            //    foreach(var itemCount in await items.ToList())
            //    {
            //        fhivm.Inventory.Add
            //        (
            //            new ItemCount
            //            {
            //                ItemCountId = itemCount.ItemCountId,
            //                Item = itemCount.Item,
            //                Count = itemCount.Count
            //            }
            //        );
            //    }
            //}
            //else
            //{
            //    foreach(var itemCount in await items.ToList())
            //    {
            //        fhivm.Inventory.Add
            //        (
            //            new ItemCount
            //            {
            //                ItemCountId = itemCount.ItemCountId,
            //                Item = itemCount.Item,
            //                Count = 0
            //            }
            //        );
            //    }
            //}
        }

        [HttpPost]
        public async Task<IActionResult> FrontHouseInventorySave(FrontHouseInventoryViewModel fhivm)
        {
            if (fhivm == null)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    //Update ItemCounts
                    foreach (var ic in fhivm.Inventory)
                    {
                        var temp = await (from i in _context.ItemCounts
                                          where i.ItemCountId == ic.ItemCountId
                                          select i).SingleOrDefaultAsync();

                        temp.Count = ic.Count;

                        _context.ItemCounts.Update(temp);
                    }


                    //Update Save Time
                    var areaLog = await (from a in _context.InventoryAreaLogs
                                         where a.InventoryAreaInventoryLogId == fhivm.Log.InventoryAreaid
                                         select a).SingleOrDefaultAsync();


                    //areaLog.Date = DateTime.Now;
                    _context.InventoryAreaLogs.Update(areaLog);

                    await _context.SaveChangesAsync();
                    //Save(fhivm);
                }
                catch (DbUpdateConcurrencyException)
                {
                    throw;
                }

                //return RedirectToAction("Index", "InventorySummary");
                return RedirectToAction("FrontHouseInventory", new { id = fhivm.Log.InventoryAreaid, loadFromContext = true });
            }

            return View(fhivm);
        }

        [HttpPost]
        public async Task<IActionResult> FrontHouseInventorySubmit(FrontHouseInventoryViewModel fhivm)
        {
            if (fhivm == null) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    foreach (var ic in fhivm.Inventory)
                    {
                        var temp = await (from i in _context.ItemCounts
                                          where i.ItemCountId == ic.ItemCountId
                                          select i).SingleOrDefaultAsync();

                        temp.Count = ic.Count;

                        _context.ItemCounts.Update(temp);
                    }

                    //Update Save Time
                    var areaLog = await (from a in _context.InventoryAreaLogs
                                         where a.InventoryAreaInventoryLogId == fhivm.Log.InventoryAreaid
                                         select a).SingleOrDefaultAsync();


                    areaLog.Date = DateTime.Now;
                    _context.InventoryAreaLogs.Update(areaLog);

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    throw;
                }

                return RedirectToAction("Submit", "InventorySummary", new { id = fhivm.Log.InventorySummaryId });
            }

            return View(fhivm);
        }

        [HttpPost]
        public async Task<IActionResult> FrontHouseInventoryReset(FrontHouseInventoryViewModel fhivm)
        {
            return RedirectToAction("FrontHouseInventory", new { id = fhivm.Log.InventoryAreaid, loadFromContext = false });
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