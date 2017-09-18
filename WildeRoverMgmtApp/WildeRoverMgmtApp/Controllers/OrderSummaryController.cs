using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using WildeRoverMgmtApp.Models;

namespace WildeRoverMgmtApp.Controllers
{
    [Authorize]
    public class OrderSummaryController : Controller
    {
        private readonly WildeRoverMgmtAppContext _context;

        public OrderSummaryController(WildeRoverMgmtAppContext context)
        {
            _context = context;
        }

        //Action to view a list of all orders
        public async Task<IActionResult> Index()
        {
            var order = from o in _context.OrderLog
                        orderby o.Date descending
                        select o;

            return View(await order.ToListAsync());
        }

        //Page to see the details of an OrderSummary
        //id - Id of OrderSummary
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();  //Validate

            //Get OrderSummary
            var order = await (from o in _context.OrderLog.Include("OrderList.Item")
                               where o.OrderSummaryId == id
                               select o).SingleOrDefaultAsync();

            if (order == null) return NotFound();  //validate

            return View(order);
        }

        //Submit page for selected OrderSummary
        //id - Id of OrderSummary
        [HttpGet]
        public async Task<IActionResult> Submit(int? id)
        {
            if (id == null) return NotFound();  //Validate

            //Get OrderSummary from context
            var order = await (from log in _context.OrderLog.Include("OrderList.Item.VendorItems.Vendor")
                               where log.OrderSummaryId == id
                               select log).SingleOrDefaultAsync();

            if (order == null) return NotFound(); //Validate

            //Create view model
            OrderSubmitViewModel osvm = new OrderSubmitViewModel();
            osvm.Order = order;
            osvm.OrderSummaryId = order.OrderSummaryId;
            osvm.Total = order.CalculateTotal();

            //Populate view model OrderItems
            foreach(var itemCount in order.OrderList)
            {
                List<ItemCount> tempList;
                string key = itemCount.Item.DefaultVendorItem.Vendor.Name;
                if (!osvm.OrderItems.TryGetValue(key, out tempList))  //Key found in view model OrderItems
                {
                    //Add new dictionary entry
                    tempList = new List<ItemCount>();
                    osvm.OrderItems.Add(key, tempList);
                }

                //Add itemCount to OrderItems
                tempList.Add(itemCount);
            }

            return View(osvm);
        }

        //Submit Post Method
        [HttpPost]
        public async Task<IActionResult> Submit(int? id, OrderSubmitViewModel osvm)
        {
            if (id == null) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    //get OrderSummary
                    var order = await (from log in _context.OrderLog.Include("OrderList.Item.VendorItems.Vendor")
                                       where log.OrderSummaryId == id
                                       select log).SingleOrDefaultAsync();

                    if (order == null) return NotFound();  //Validate


                    //====EMAIL VENDORS HERE===================================




                    //=========================================================

                    //Update OrderSummary
                    order.Completed = true;
                    order.Total = osvm.Total;

                    _context.OrderLog.Update(order);  //update context

                    await _context.SaveChangesAsync();  //save context

                    return RedirectToAction("Index");

                }
                catch (DbUpdateConcurrencyException)
                {
                    throw;
                }
            }
            else
            {
                return RedirectToAction("Index");
            }
        }
    }
}