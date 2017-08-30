using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WildeRoverMgmtApp.Models;
using Microsoft.EntityFrameworkCore;


namespace WildeRoverMgmtApp.Controllers
{
    public class InventoryAreaController : Controller
    {
        private readonly WildeRoverMgmtAppContext _context;

        public InventoryAreaController(WildeRoverMgmtAppContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var areas = from a in _context.InventoryAreas
                        orderby a.House descending, a.Name ascending
                        select a;

            return View(await areas.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var area = await (from a in _context.InventoryAreas.Include("ItemSlots.WildeRoverItem")
                              where a.InventoryAreaId == id
                              select a).SingleOrDefaultAsync();

            if (area == null) return NotFound();

            return View(area);
        }

        [HttpGet]
        public async Task<IActionResult>Edit(int? id)
        {
            if (id == null) return NotFound();

            var area = await (from a in _context.InventoryAreas.Include("ItemSlots.WildeRoverItem")
                              where a.InventoryAreaId == id
                              select a).SingleOrDefaultAsync();

            if (area == null) return NotFound();

            var items = await (from i in _context.WildeRoverItem
                               orderby i.ItemHouse, i.Name
                               select i).ToListAsync();

            var slots = area.ItemSlots.OrderBy(t => t.Slot);

            List<SelectListItem> selectList = new List<SelectListItem>();
            selectList.Add
            (
                new SelectListItem
                {
                    Value = 0.ToString(),
                    Text = "<Select Item>"
                }
            );
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

            InventoryAreaEditViewModel model = new InventoryAreaEditViewModel();
            model.InventoryAreaId = area.InventoryAreaId;
            model.InventoryArea = area;
            model.ItemList = selectList;

            model.SlotList = new List<InventoryAreaEditEntry>();
            foreach(var slot in slots)
            {
                InventoryAreaEditEntry entry = new InventoryAreaEditEntry();
                entry.InventoryAreaId = area.InventoryAreaId;
                entry.InventorySlotId = slot.InventorySlotId;
                entry.Slot = slot;
                //entry.ItemList = selectList;                
                if (slot.WildeRoverItemId == null)
                {
                    entry.WildeRoverItemId = 0;
                }
                else
                {
                    entry.WildeRoverItemId = (int)slot.WildeRoverItemId;
                }

                //entry.selectList = new SelectList(items, "WildeRoverItemId", "Name");

                model.SlotList.Add(entry);
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(InventoryAreaEditViewModel model)
        {
            if (model == null) return NotFound();

            if (ModelState.IsValid)
            {   
                try
                {
                    var area = await (from a in _context.InventoryAreas.Include("ItemSlots.WildeRoverItem")
                                      where a.InventoryAreaId == model.InventoryAreaId
                                      select a).SingleOrDefaultAsync();

                    if (area == null) return NotFound();

                    var slotDict = area.ItemSlots.ToDictionary(t => t.InventorySlotId);

                    foreach(var entry in model.SlotList)
                    {
                        InventorySlot temp = slotDict[entry.InventorySlotId];
                        if (entry.WildeRoverItemId != 0)
                        {
                            temp.WildeRoverItemId = entry.WildeRoverItemId;
                            _context.Slots.Update(temp);
                        }
;                   }

                    await _context.SaveChangesAsync();

                    return RedirectToAction("Details", new { id = model.InventoryAreaId });
                }
                catch (DbUpdateConcurrencyException)
                {
                    throw;
                }
            }

            return View(model);
        }

        public async Task<IActionResult> AddNewSlot(int id)
        {
            var area = await (from a in _context.InventoryAreas.Include("ItemSlots")
                              where a.InventoryAreaId == id
                              select a).SingleOrDefaultAsync();

            if (area == null) return NotFound();

            InventorySlot slot = new InventorySlot();
            slot.InventoryAreaId = area.InventoryAreaId;
            slot.InventoryArea = area;
            slot.Slot = area.ItemSlots.Count + 1;
            //slot.WildeRoverItemId = 0;

            _context.Slots.Add(slot);

            area.ItemSlots.Add(slot);
            _context.InventoryAreas.Update(area);

            await _context.SaveChangesAsync();

            return RedirectToAction("Edit", new { id = id });
        }

        public async Task<IActionResult> RemoveSlot(int areaId, int slotId)
        {
            var area = await (from a in _context.InventoryAreas.Include("ItemSlots")
                              where a.InventoryAreaId == areaId
                              select a).SingleOrDefaultAsync();

            if (area == null) return NotFound();

            var removeSlot = (from s in area.ItemSlots
                              where s.InventorySlotId == slotId
                              select s).SingleOrDefault();

            if (removeSlot == null) return NotFound();

            int slotNumber = removeSlot.Slot;

            foreach(var s in area.ItemSlots)
            {
                if (s.Slot > slotNumber)
                {
                    s.Slot--;
                    _context.Update(s);
                }                    
            }

            _context.Slots.Remove(removeSlot);

            await _context.SaveChangesAsync();

            return RedirectToAction("Edit", new { id = areaId });
        }
    }
}