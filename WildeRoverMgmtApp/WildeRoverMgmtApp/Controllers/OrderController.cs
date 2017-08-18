using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WildeRoverMgmtApp.Models;
using Microsoft.EntityFrameworkCore;

namespace WildeRoverMgmtApp.Controllers
{   
    public class OrderController : Controller
    {
        private readonly WildeRoverMgmtAppContext _context;
        private OrderSummary _order;

        public OrderController(WildeRoverMgmtAppContext context)
        {
            _context = context;

            var current = (from o in _context.OrderLog.Include(m => m.OrderList)
                           orderby o.Date descending
                           select o).FirstOrDefault();

            if (current == null || current.Completed)
            {
                //Create new OrderSummary
                current = new OrderSummary();
                current.Completed = false;
                current.Date = DateTime.Now;

                _context.OrderLog.Add(current);
                _context.SaveChanges();

                current = (from o in _context.OrderLog
                           orderby o.Date descending
                           select o).FirstOrDefault();
            }

            _order = current;
        }

        //GET
        [HttpGet]
        public async Task<IActionResult> FrontHouseOrder(bool? loadFromContext)
        {
            
            var items = await (from i in _context.WildeRoverItem.Include(m => m.VendorItems)
                               where i.ItemHouse.HasFlag(WildeRoverItem.House.front)
                               orderby i.Type, i.SubType, i.Name
                               select i).ToListAsync();

            OrderViewModel ovm = new OrderViewModel();
            ovm.OrderId = _order.OrderSummaryId;

            foreach(var item in items)
            {
                ItemCount temp = new ItemCount();
                temp.Item = item;
                temp.WildeRoverItemId = item.WildeRoverItemId;

                if (loadFromContext == false)
                {
                    temp.Count = 0;
                }
                else
                {
                    temp.Count = temp.Item.Need;
                }
                

                ovm.OrderList.Add(temp);

            }

            return View(ovm);            
        }

        //POST
        [HttpPost]
        public async Task<IActionResult> FrontHouseOrder(OrderViewModel ovm)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    foreach (var itemCount in ovm.OrderList)
                    {
                        //Find existing itemCount from _order
                        var temp = (from ic in _order.OrderList
                                    where itemCount.WildeRoverItemId == ic.WildeRoverItemId
                                    select ic).SingleOrDefault();

                        if (itemCount.Count > 0)
                        {
                            if (temp == null)
                            {
                                //Add ItemCount
                                temp = new ItemCount();
                                temp.OrderSummary = _order;
                                temp.OrderSummaryId = _order.OrderSummaryId;
                                temp.WildeRoverItemId = itemCount.WildeRoverItemId;
                                temp.Count = itemCount.Count;

                                _context.ItemCounts.Add(temp);

                                _order.OrderList.Add(temp);
                            }
                            else
                            {
                                //Update ItemCount
                                temp.Count = itemCount.Count;

                                _context.ItemCounts.Update(temp);
                            }

                        }
                        else if (itemCount.Count == 0)
                        {
                            if (temp != null)
                            {
                                //Remove from OrderList
                                _order.OrderList.Remove(temp);

                                _context.ItemCounts.Remove(temp);
                            }
                        }
                    }
                    

                    _context.Update(_order);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    throw;
                }

                return RedirectToAction("Index", "OrderSummary");
            }

            return View(ovm);
        }        

        public async Task<IActionResult> Reset()
        {
            return RedirectToAction("FrontHouseOrder", new { loadFromContext = false });
        }
    }
}