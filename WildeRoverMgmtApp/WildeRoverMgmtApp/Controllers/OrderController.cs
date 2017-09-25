using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using WildeRoverMgmtApp.Models;
using Microsoft.EntityFrameworkCore;

namespace WildeRoverMgmtApp.Controllers
{   
    [Authorize]
    public class OrderController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly WildeRoverMgmtAppContext _context;
        private OrderSummary _order;

        public OrderController(WildeRoverMgmtAppContext context, UserManager<User> userManager)
        {
            _userManager = userManager;
            _context = context;

            //Get latest OrderSummary  (TODO:  change to also query for user as well)
            var current = (from o in _context.OrderLog.Include("OrderList.Item")
                           orderby o.Date descending
                           select o).FirstOrDefault();

            //If there is no uncompleted order
            if (current == null || current.Completed)
            {
                //Create new OrderSummary
                current = new OrderSummary();
                current.Completed = false;
                current.Date = DateTime.Now;

                _context.OrderLog.Add(current);
                _context.SaveChanges();

                current = (from o in _context.OrderLog.Include("OrderList.Item")
                           orderby o.Date descending
                           select o).FirstOrDefault();
            }

            _order = current;
        }

        //Front House ORder Page
        //loadFromContext - If true, load count values from context, otherwise set values to 0
        [HttpGet]
        public async Task<IActionResult> FrontHouseOrder(bool? loadFromContext)
        {
            //Get FrontHouse Items
            var items = await (from i in _context.WildeRoverItem.Include(m => m.VendorItems)
                               where i.ItemHouse.HasFlag(WildeRoverItem.House.front)
                               orderby i.Type, i.SubType, i.Name
                               select i).ToListAsync();

            //Create View Model
            OrderViewModel ovm = new OrderViewModel();
            ovm.OrderId = _order.OrderSummaryId;

            //Populate View Model Order List
            foreach(var item in items)
            {
                //Create Item Count
                ItemCount temp = new ItemCount();
                temp.Item = item;
                temp.WildeRoverItemId = item.WildeRoverItemId;

                //If loadFromContext is false set count to 0
                if (loadFromContext == false)
                {
                    temp.Count = 0;
                }
                else  //load count from context
                {
                    temp.Count = temp.Item.Need;
                }                

                ovm.OrderList.Add(temp);  //add to view model
            }

            return View(ovm);            
        }

        //Post Method for FrontHouseOrder
        [HttpPost]
        public async Task<IActionResult> Save(OrderViewModel ovm)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    //Create dictionary of orderlist for O(1) access
                    var itemCountDict = _order.OrderList.ToDictionary(t => t.WildeRoverItemId);

                    foreach (var itemCount in ovm.OrderList)
                    {
                        //Find existing itemCount from _order

                        ItemCount temp;
                        if (itemCountDict.TryGetValue(itemCount.WildeRoverItemId, out temp))  //ItemCount found
                        {
                            if (itemCount.Count > 0)  //something is ordered
                            {
                                //Update ItemCount in context
                                temp.Count = itemCount.Count;
                                _context.ItemCounts.Update(temp);
                            }
                            else  //Item found but now its count is 0
                            {
                                //Remove ItemCount from context
                                _order.OrderList.Remove(temp);
                                _context.ItemCounts.Remove(temp);
                            }
                        }
                        else  //ItemCount does not exist in context
                        {
                            if (itemCount.Count > 0)
                            {
                                //Create ItemCount
                                temp = new ItemCount();
                                temp.OrderSummary = _order;
                                temp.OrderSummaryId = _order.OrderSummaryId;
                                temp.WildeRoverItemId = itemCount.WildeRoverItemId;
                                temp.Count = itemCount.Count;

                                //Add to context
                                _context.ItemCounts.Add(temp);

                                //Add to OrderSummary
                                _order.OrderList.Add(temp);                            
                            }
                        }
                    }

                    //Update Last Edited
                    var user = await _userManager.GetUserAsync(User);
                    if (user == null) throw new InvalidOperationException();

                    _order.LastEdited = user.FullName;

                    //Update context
                    _context.OrderLog.Update(_order);
                    await _context.SaveChangesAsync();  //save context
                }
                catch (DbUpdateConcurrencyException)
                {
                    throw;
                }

                return RedirectToAction("Index", "OrderSummary");
            }

            return View(ovm);
        }        

        //Reset Post action
        //[HttpPost]
        public IActionResult FrontHouseOrderReset()
        {
            //Redirect to FrontHouse setting loadFromContext to false
            return RedirectToAction("FrontHouseOrder", new { loadFromContext = false });
        }

        //Submit Post action
        [HttpPost]
        public async Task<IActionResult> Submit(OrderViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    //Save order before submitting

                    //Create dictionary of orderlist for O(1) access
                    var itemCountDict = _order.OrderList.ToDictionary(t => t.WildeRoverItemId);

                    foreach (var itemCount in model.OrderList)
                    {
                        //Find existing itemCount from _order
                        ItemCount temp;
                        if (itemCountDict.TryGetValue(itemCount.WildeRoverItemId, out temp))  //ItemCount found
                        {
                            if (itemCount.Count > 0)  //something is ordered
                            {
                                //Update ItemCount in context
                                temp.Count = itemCount.Count;
                                _context.ItemCounts.Update(temp);
                            }
                            else  //Item found but now its count is 0
                            {
                                //Remove ItemCount from context
                                _order.OrderList.Remove(temp);
                                _context.ItemCounts.Remove(temp);
                            }
                        }
                        else  //ItemCount does not exist in context
                        {
                            if (itemCount.Count > 0)
                            {
                                //Create ItemCount
                                temp = new ItemCount();
                                temp.OrderSummary = _order;
                                temp.OrderSummaryId = _order.OrderSummaryId;
                                temp.WildeRoverItemId = itemCount.WildeRoverItemId;
                                temp.Count = itemCount.Count;

                                //Add to context
                                _context.ItemCounts.Add(temp);

                                //Add to OrderSummary
                                _order.OrderList.Add(temp);
                            }
                        }
                    }

                    //Update Last Edited
                    var user = await _userManager.GetUserAsync(User);
                    if (user == null) throw new InvalidOperationException();

                    _order.LastEdited = user.FullName;

                    //Update context
                    _context.OrderLog.Update(_order);
                    await _context.SaveChangesAsync();  //save context
                }
                catch (DbUpdateConcurrencyException)
                {
                    throw;
                }

                //Redirect to Submit Page in OrderSummary
                return RedirectToAction("Submit", "OrderSummary", new { id = model.OrderId });
            }

            return View(model);
        }

        //Back House Order Page
        //loadFromContext - If true, load count values from context, otherwise set values to 0
        [HttpGet]
        public async Task<IActionResult> BackHouseOrder(bool? loadFromContext)
        {
            //Get FrontHouse Items
            var items = await (from i in _context.WildeRoverItem.Include(m => m.VendorItems)
                               where i.ItemHouse.HasFlag(WildeRoverItem.House.back)
                               orderby i.Type, i.SubType, i.Name
                               select i).ToListAsync();

            //Create View Model
            OrderViewModel ovm = new OrderViewModel();
            ovm.OrderId = _order.OrderSummaryId;

            //Populate View Model Order List
            foreach (var item in items)
            {
                //Create Item Count
                ItemCount temp = new ItemCount();
                temp.Item = item;
                temp.WildeRoverItemId = item.WildeRoverItemId;

                //If loadFromContext is false set count to 0
                if (loadFromContext == false)
                {
                    temp.Count = 0;
                }
                else  //load count from context
                {
                    temp.Count = temp.Item.Need;
                }

                ovm.OrderList.Add(temp);  //add to view model
            }

            return View(ovm);
        }
    }  
}