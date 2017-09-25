using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using WildeRoverMgmtApp.Models;
using Microsoft.EntityFrameworkCore;


namespace WildeRoverMgmtApp.Controllers
{
    [Authorize]
    public class InventoryAreaController : Controller
    {
        private readonly WildeRoverMgmtAppContext _context;

        public InventoryAreaController(WildeRoverMgmtAppContext context)
        {
            _context = context;
        }

        //A view of all InventoryAreas in database
        public async Task<IActionResult> Index()
        {
            var areas = from a in _context.InventoryAreas
                        orderby a.House descending, a.Name ascending
                        select a;

            return View(await areas.ToListAsync());
        }

        //Detail view for a specific Inventory area
        //id - id for InventoryArea
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var area = await (from a in _context.InventoryAreas.Include("ItemSlots.WildeRoverItem")
                              where a.InventoryAreaId == id
                              select a).SingleOrDefaultAsync();

            if (area == null) return NotFound();

            return View(area);
        }

        //Edit an InventoryArea
        //id - id for InventoryArea
        [HttpGet]
        public async Task<IActionResult> Edit(int? id, int? displayCount)
        {
            if (id == null) return NotFound();

            //get area from context
            var area = await (from a in _context.InventoryAreas.Include("ItemSlots.WildeRoverItem")
                              where a.InventoryAreaId == id
                              select a).SingleOrDefaultAsync();

            if (area == null) return NotFound();  //validate

            //get WildeRoverItems to populate selectlist
            var items = await (from i in _context.WildeRoverItem
                               orderby i.ItemHouse, i.Name
                               select i).ToListAsync();

            //order InventorySlots in area by their number
            var slots = area.ItemSlots.OrderBy(t => t.Slot).ToList();

            //Create selection list
            //List<SelectListItem> selectList = new List<SelectListItem>();
            //selectList.Add
            //(
            //    //Selection if nothing is selected
            //    new SelectListItem
            //    {
            //        Value = 0.ToString(),
            //        Text = "<Select Item>"
            //    }
            //);
            ////Create selections for every WildeRoverItem
            //foreach (var item in items)
            //{
            //    selectList.Add
            //    (
            //        new SelectListItem
            //        {
            //            Value = item.WildeRoverItemId.ToString(),
            //            Text = item.Name
            //        }
            //    );
            //}

            //Create view model
            InventoryAreaEditViewModel model = new InventoryAreaEditViewModel();
            model.InventoryAreaId = area.InventoryAreaId;
            model.InventoryArea = area;
            //model.ItemList = selectList;  //assign select list

            //Populate Slots
            model.SlotList = new List<InventoryAreaEditEntry>();

            if (displayCount == null)
            {
                displayCount = slots.Count;
            }

            for (int i = 0; i < displayCount; i++)
            {
                InventoryAreaEditEntry entry = new InventoryAreaEditEntry();
                entry.InventoryAreaId = area.InventoryAreaId;
                if (i < slots.Count)
                {
                    entry.InventorySlotId = slots[i].InventorySlotId;
                    entry.Slot = slots[i].Slot;

                    if (slots[i].WildeRoverItemId == null)
                    {
                        entry.WildeRoverItemId = 0;
                    }
                    else
                    {
                        entry.WildeRoverItemId = (int)slots[i].WildeRoverItemId;
                    }
                }
                else
                {
                    entry.WildeRoverItemId = 0;
                    entry.Slot = i + 1;
                }

                model.SlotList.Add(entry);
            }

            model.SlotDisplayCount = (int)displayCount;

            return View(model);
        }

        //Save Post method for Edit
        [HttpPost]
        public async Task<IActionResult> EditSave(InventoryAreaEditViewModel model)
        {
            if (model == null) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    //Get area from context
                    var area = await (from a in _context.InventoryAreas.Include("ItemSlots.WildeRoverItem")
                                      where a.InventoryAreaId == model.InventoryAreaId
                                      select a).SingleOrDefaultAsync();

                    if (area == null) return NotFound();  //Validate

                    //Create dictionary of ItemSlots for O(1) updates
                    var slotDict = area.ItemSlots.ToDictionary(t => t.InventorySlotId);

                    //Loop through model.ItemSlots and update context
                    int i = 0;
                    while (i < model.SlotDisplayCount || i < area.ItemSlots.Count)
                    {
                        if (i < area.ItemSlots.Count && i < model.SlotDisplayCount)  //Update
                        {
                            var temp = slotDict[model.SlotList[i].InventorySlotId];
                            if (model.SlotList[i].WildeRoverItemId == 0)
                            {
                                temp.WildeRoverItemId = null;
                            }
                            else
                            {
                                temp.WildeRoverItemId = model.SlotList[i].WildeRoverItemId;
                            }

                            _context.Slots.Update(temp);
                        }
                        else if (i < model.SlotDisplayCount && i >= area.ItemSlots.Count)  //Add
                        {
                            InventorySlot newSlot = new InventorySlot();
                            newSlot.InventoryAreaId = area.InventoryAreaId;
                            newSlot.Slot = i + 1;
                            if (model.SlotList[i].WildeRoverItemId != 0)
                            {
                                newSlot.WildeRoverItemId = model.SlotList[i].WildeRoverItemId;
                            }

                            _context.Slots.Add(newSlot);
                        }
                        else if (i >= model.SlotDisplayCount && i < area.ItemSlots.Count)  //Remove
                        {
                            var toRemove = area.ItemSlots[i];
                            area.ItemSlots.RemoveAt(i);

                            _context.Slots.Remove(toRemove);
                        }

                        i++;
                    }

                    await _context.SaveChangesAsync();  //Save context

                    return RedirectToAction("Details", new { id = model.InventoryAreaId });
                }
                catch (DbUpdateConcurrencyException)
                {
                    throw;
                }
            }

            return View(model);
        }

        //Cancel post method for edit
        [HttpPost]
        public IActionResult EditCancel(InventoryAreaEditViewModel model)
        {
            //redirect back to details page for inventory area
            return RedirectToAction("Details", new { id = model.InventoryAreaId });
        }

        //Add a new InventorySlot to InventoryArea
        //id - Id of InventoryArea
        //displayCount - current amount of slots to display in edit page
        public IActionResult AddNewSlot(int id, int displayCount)
        {
            return RedirectToAction("Edit", new { id = id, displayCount = displayCount + 1 });
        }

        //Remove one InventorySlot
        //id - Id of InventoryArea
        //displayCount - current amount of slots to display in edit page
        public IActionResult RemoveSlot(int id, int displayCount)
        {
            return RedirectToAction("Edit", new { id = id, displayCount = displayCount - 1 });
        }         

        //Create InventoryArea page
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        //Post for Create
        //areaName - Name of the Inventory area
        //house - WildeRoverItem.House enum flag
        [HttpPost]
        public async Task<IActionResult> Create(string areaName, int house)
        {
            if (ModelState.IsValid)
            {
                InventoryArea newArea = new InventoryArea
                {
                    Name = areaName
                };

                if ((WildeRoverItem.House)house == WildeRoverItem.House.both)
                {
                    newArea.House = WildeRoverItem.House.front | WildeRoverItem.House.back | WildeRoverItem.House.both;
                }

                _context.InventoryAreas.Add(newArea);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index", "InventoryArea");
                    
            }

            return View();
        }
        
        //Delete
        [HttpGet]
        public Task<IActionResult> Delete(int id)
        {
            return DeleteConfirmed(id);
        }

        //Post method for Delete
        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var area = await (from a in _context.InventoryAreas
                        where a.InventoryAreaId == id
                        select a).SingleOrDefaultAsync();

            if (area == null) return NotFound();

            _context.InventoryAreas.Remove(area);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "InventoryArea");
        }
    }
}