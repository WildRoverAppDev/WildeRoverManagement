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
        public async Task<IActionResult>Edit(int? id)
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
            var slots = area.ItemSlots.OrderBy(t => t.Slot);

            //Create selection list
            List<SelectListItem> selectList = new List<SelectListItem>();
            selectList.Add
            (
                //Selection if nothing is selected
                new SelectListItem
                {
                    Value = 0.ToString(),
                    Text = "<Select Item>"
                }
            );
            //Create selections for every WildeRoverItem
            foreach(var item in items)
            {
                selectList.Add
                (
                    new SelectListItem
                    {
                        Value = item.WildeRoverItemId.ToString(),
                        Text = item.Name                        
                    }
                );
            }

            //Create view model
            InventoryAreaEditViewModel model = new InventoryAreaEditViewModel();
            model.InventoryAreaId = area.InventoryAreaId;
            model.InventoryArea = area;
            model.ItemList = selectList;  //assign select list

            //Populate Slots
            model.SlotList = new List<InventoryAreaEditEntry>();
            foreach(var slot in slots)
            {
                InventoryAreaEditEntry entry = new InventoryAreaEditEntry();
                entry.InventoryAreaId = area.InventoryAreaId;
                entry.InventorySlotId = slot.InventorySlotId;
                entry.Slot = slot;
                
                //If Slot has no WildeRoverItem, set its WildeRoverItemId to 0
                if (slot.WildeRoverItemId == null)
                {
                    entry.WildeRoverItemId = 0;
                }
                else
                {
                    entry.WildeRoverItemId = (int)slot.WildeRoverItemId;
                }               

                model.SlotList.Add(entry);
            }

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

                    //Update Slots with data from view model
                    foreach(var entry in model.SlotList)
                    {
                        //Only update if Slot has a WildeRoverItem assigned
                        if (entry.WildeRoverItemId != 0)
                        {
                            InventorySlot temp = slotDict[entry.InventorySlotId];
                            temp.WildeRoverItemId = entry.WildeRoverItemId;
                            _context.Slots.Update(temp);
                        }
;                   }

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
        public async Task<IActionResult> EditCancel(InventoryAreaEditViewModel model)
        {
            //redirect back to details page for inventory area
            return RedirectToAction("Details", new { id = model.InventoryAreaId });
        }

        //Add a new InventorySlot to INventoryArea
        //id - Id of InventoryArea
        public async Task<IActionResult> AddNewSlot(int id)
        {
            //get InventoryArea
            var area = await (from a in _context.InventoryAreas.Include("ItemSlots")
                              where a.InventoryAreaId == id
                              select a).SingleOrDefaultAsync();

            if (area == null) return NotFound();  //Validate

            //Create new Inventory Slot
            InventorySlot slot = new InventorySlot();
            slot.InventoryAreaId = area.InventoryAreaId;
            slot.InventoryArea = area;
            slot.Slot = area.ItemSlots.Count + 1;            

            //Add InventorySlot to context
            _context.Slots.Add(slot);
            
            //Add InventorySlot to InventoryArea
            area.ItemSlots.Add(slot);
            _context.InventoryAreas.Update(area);

            await _context.SaveChangesAsync();  //Save context

            //Refresh Edit page
            return RedirectToAction("Edit", new { id = id });
        }

        //Remove slot from InventoryArea
        //areaId - Id of InventoryArea
        //slotId - Id for InventorySlot to be removed
        public async Task<IActionResult> RemoveSlot(int areaId, int slotId)
        {
            //Get InventoryArea from context
            var area = await (from a in _context.InventoryAreas.Include("ItemSlots")
                              where a.InventoryAreaId == areaId
                              select a).SingleOrDefaultAsync();

            if (area == null) return NotFound();  //Validate

            //Get InventorySlot from the InventoryArea
            var removeSlot = (from s in area.ItemSlots
                              where s.InventorySlotId == slotId
                              select s).SingleOrDefault();

            if (removeSlot == null) return NotFound();  //Validate

            int slotNumber = removeSlot.Slot;  //get the Slot number

            //Update other InventorySlots in InventoryArea to get rid of empty slot
            //Slide all slots after the removed slot up
            foreach(var s in area.ItemSlots)
            {
                if (s.Slot > slotNumber)
                {
                    s.Slot--;
                    _context.Update(s);
                }                    
            }

            //Delete InventorySlot from context
            _context.Slots.Remove(removeSlot);

            //Save Context
            await _context.SaveChangesAsync();

            //Refresh Edit page
            return RedirectToAction("Edit", new { id = areaId });
        }
    }
}