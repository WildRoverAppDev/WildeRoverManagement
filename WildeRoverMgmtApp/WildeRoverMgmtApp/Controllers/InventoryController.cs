using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WildeRoverMgmtApp.Models;

namespace WildeRoverMgmtApp.Controllers
{
    [Authorize]
    public class InventoryController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly WildeRoverMgmtAppContext _context;
        private InventorySummary _summary;

        public InventoryController(WildeRoverMgmtAppContext context, UserManager<User> userManager)
        {
            _userManager = userManager;
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

                _context.InventoryLog.Update(_summary);
                

                CreateInventoryAreaLogs(_summary);

                _context.SaveChanges();
            }

            else  //Log exists
            {
                _summary = current;
            }

            
        }

        //Create InventoryAreaLogs associated with new Inventory Summary
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

        //Main View
        public IActionResult Index()
        {
            return View();
        }

        //Menu of Front House Inventory Areas
        public async Task<IActionResult> FrontHouseIndex()
        {
            var areas = await (from a in _context.InventoryAreas
                         select a).ToListAsync();

            return View(areas);
        }
              
        //Action to perform Front of House Inventory
        //id - Id of InventoryAreaLog
        //loadFromContext - if true, preload view model with count data from context,
        //                  otherwise set counts to 0
        [HttpGet]
        public async Task<IActionResult> FrontHouseInventory(int id, bool? loadFromContext)
        {
            //get inventory area log
            var areaLog = (from al in _summary.InventoryAreaLogs
                           where al.InventoryAreaid == id
                           select al).SingleOrDefault();

            if (areaLog == null) return NotFound();
            
            //Create View Model
            FrontHouseInventoryViewModel fhivm = new FrontHouseInventoryViewModel();
            fhivm.Log = areaLog;

            //Load ViewModel with item counts
            foreach(var ic in areaLog.Inventory)
            {
                ItemCount temp = new ItemCount();
                temp.ItemCountId = ic.ItemCountId;
                temp.Item = ic.Item;

                //Set count value based on value of loadFromContext
                if (loadFromContext == null || loadFromContext == true)
                {
                    temp.Count = ic.Count;  //get value from context
                }
                else
                {
                    temp.Count = 0;  //set count value to 0
                }

                //Add to itemCount to view model
                fhivm.Inventory.Add(temp);
            }

            return View(fhivm);
        }

        //Save Post method for FrontHouseInventory
        //fhivm - View model
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
                        //get corresponding itemCount from context
                        var temp = await (from i in _context.ItemCounts
                                          where i.ItemCountId == ic.ItemCountId
                                          select i).SingleOrDefaultAsync();

                        if (temp == null) continue;

                        temp.Count = ic.Count;  //update count

                        _context.ItemCounts.Update(temp);  //update context
                    }

                    //Update Save Time

                    //get area log from context
                    var areaLog = await (from a in _context.InventoryAreaLogs
                                         where a.InventoryAreaInventoryLogId == fhivm.Log.InventoryAreaid
                                         select a).SingleOrDefaultAsync();

                    areaLog.Date = DateTime.Now;
                    _context.InventoryAreaLogs.Update(areaLog);

                    //Update Inventory Summary last edited user
                    var inventorySummary = await (from log in _context.InventoryLog
                                                  where log.InventorySummaryId == areaLog.InventorySummaryId
                                                  select log).SingleOrDefaultAsync();

                    var user = await _userManager.GetUserAsync(User);

                    inventorySummary.LastEdited = user.FullName;
                    _context.InventoryLog.Update(inventorySummary);

                    //Save context
                    await _context.SaveChangesAsync();
                    
                }
                catch (DbUpdateConcurrencyException)
                {
                    throw;
                }

                return RedirectToAction("FrontHouseIndex");
                //return RedirectToAction("FrontHouseInventory", new { id = fhivm.Log.InventoryAreaid, loadFromContext = true });
            }

            return RedirectToAction("FrontHouseInventory", new { id = fhivm.Log.InventoryAreaid, loadFromContext = true });
        }

        //Submit Post for FrontHouseInventory
        [HttpPost]
        public async Task<IActionResult> FrontHouseInventorySubmit(FrontHouseInventoryViewModel fhivm)
        {
            if (fhivm == null) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    //Save ItemCounts
                    foreach (var ic in fhivm.Inventory)
                    {
                        //get corresponding itemcount from context
                        var temp = await (from i in _context.ItemCounts
                                          where i.ItemCountId == ic.ItemCountId
                                          select i).SingleOrDefaultAsync();

                        temp.Count = ic.Count;

                        //update context
                        _context.ItemCounts.Update(temp);
                    }

                    //Update Save Time

                    //get area log from context
                    var areaLog = await (from a in _context.InventoryAreaLogs
                                         where a.InventoryAreaInventoryLogId == fhivm.Log.InventoryAreaid
                                         select a).SingleOrDefaultAsync();
                    
                    areaLog.Date = DateTime.Now;
                    
                    //update arealog
                    _context.InventoryAreaLogs.Update(areaLog);

                    //Update Inventory Summary last edited user
                    var inventorySummary = await (from log in _context.InventoryLog
                                                  where log.InventorySummaryId == areaLog.InventorySummaryId
                                                  select log).SingleOrDefaultAsync();

                    var user = await _userManager.GetUserAsync(User);

                    inventorySummary.LastEdited = user.FullName;
                    _context.InventoryLog.Update(inventorySummary);

                    //save context
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    throw;
                }

                //Redirect to Submit page
                return RedirectToAction("Submit", "InventorySummary", new { id = fhivm.Log.InventorySummaryId });
            }

            return RedirectToAction("FrontHouseInventory", new { id = fhivm.Log.InventoryAreaid, loadFromContext = true });
        }

        //Reset Post method for FrontHouseInventory
        [HttpPost]
        public  IActionResult FrontHouseInventoryReset(FrontHouseInventoryViewModel fhivm)
        {
            //Redirect to FrontHouseInventory, setting loadFromContext to false
            return RedirectToAction("FrontHouseInventory", new { id = fhivm.Log.InventoryAreaid, loadFromContext = false });
        }

        [HttpGet]
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