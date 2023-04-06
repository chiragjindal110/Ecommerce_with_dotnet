using Ecommerce.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Reflection.Metadata;
using System.Text.Json;

namespace Ecommerce.Controllers
{
    
    public class CheckoutController : Controller
    {
        private readonly EcommerceContext db;
        private bool locked = false;

        public CheckoutController(EcommerceContext _db)
        {
            db = _db;
        }
        public IActionResult checkout()
        {
            if(string.IsNullOrEmpty(HttpContext.Session.GetString("email")))
            {
                return RedirectToAction("Home", "Home");
            }
                else if (HttpContext.Session.GetString("isVerified")!=null && HttpContext.Session.GetString("isVerified") =="false")
            {
                return RedirectToAction("verifyMail", "Index");
            }
            return View("checkout",HttpContext.Session.GetString("address"));
        }
        [HttpPost]
        public string getproduct([FromBody]JsonDocument doc)
        {
            int id = doc.RootElement.GetProperty("product_id").GetInt32();
            var output = db.Products.Where(p => p.ProductId == id).ToList();
            return JsonConvert.SerializeObject(output[0]);
        }

        [HttpPost]
        public string storeaddress([FromBody]JsonDocument doc)
        {
            string address = doc.RootElement.GetProperty("address").GetString();
            var obj = db.Users.FirstOrDefault(u => u.UserId == HttpContext.Session.GetInt32("user_id"));
            if (obj != null)
            {
                obj.Address = address;
                db.SaveChanges();
                return JsonConvert.SerializeObject(new {status = true});
            }
            return JsonConvert.SerializeObject(new { status = false});
        }

        [HttpPost]
        public string placeorder([FromBody]JsonDocument doc)
        {
            var products = JArray.Parse(doc.RootElement.GetProperty("products").ToString(),null);
            Console.WriteLine(products[0]);
            if (!locked)
            {
                locked = true;
                var transaction = db.Database.BeginTransaction();
                try
                {
                    foreach (var product in products)
                    {
                        var id = Convert.ToInt32(product["ProductId"]);
                        var quantity = Convert.ToInt32(product["Quantity"]);
                        var stock = db.Products.Where(p => p.ProductId == id).Select(p => p.Stock).ToList();
                        if (stock.Any())
                        {
                            if (stock[0] >= quantity )
                            {
                                var temp = db.Products.FirstOrDefault(p => p.ProductId == id);
                                if(temp != null)
                                {
                                    if (temp.Stock >= quantity)
                                    {
                                        temp.Stock = temp.Stock - quantity;
                                        temp.Sale = temp.Sale + quantity;
                                        db.Orders.Add(new Order 
                                        { 
                                            ProductId = id, 
                                            Quantity = quantity, 
                                            ItemValue = quantity * (int)temp.Price, 
                                            CustomerAddress = HttpContext.Session.GetString("address"), 
                                            Status = "{\"status\":\"Placed\"}", 
                                            UserId = HttpContext.Session.GetInt32("user_id") 
                                        });
                                        var cart = db.Carts.FirstOrDefault(p => p.ProductId == id && p.UserId == HttpContext.Session.GetInt32("user_id"));
                                        if(cart != null)
                                        {
                                        db.Carts.Remove(cart);
                                        }
                                        db.SaveChanges();
                                    }
                                    else { throw new Exception(); }
                                }
                                else { throw new Exception(); }
                            }
                            else { throw new Exception(); }
                        }
                        else { throw new Exception(); }
                    }
                    transaction.Commit();
                    locked = false;
                    return JsonConvert.SerializeObject(new { message = "Order Placed Successfully" });
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    transaction.Rollback();
                    locked = false;
                    return JsonConvert.SerializeObject(new { message = "OOPS!! Something Went Wrong, Try placing order Again" });
                }

            }
            else
            {
                return placeorder(doc);
            }
        }
    }

}
