
using Ecommerce.Models;
using Mailjet.Client.Resources;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using NuGet.Protocol;
using System.Text.Json;

namespace Ecommerce.Controllers
{
    public class HomeController : Controller
    {
        private readonly EcommerceContext db;

        public HomeController(EcommerceContext _db)
        {
            db = _db;
        }

        public IActionResult Home()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("email")))
            {
            return View(false) ;
            }
            else
            {
                return View(true);
            }
        }

        [HttpGet] public string isLogin()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("email")))
            {
                return JsonConvert.SerializeObject(new { flag = false });
            }
            else
            {
                return JsonConvert.SerializeObject(new { flag = true });
            }
        }

        [HttpPost]
        public string getProducts([FromBody]JsonDocument start)
        {
            var s = start.RootElement.GetProperty("start").GetInt32();
            var result = db.Products
            .Where(p => p.Status == 1)
            .OrderBy(p => p.ProductId)
            .Skip(s)
            .Take(5)
            .ToList();

            var json = JsonConvert.SerializeObject(result);
            return json;
        }

        [HttpGet]
        public IActionResult Logout() {
            HttpContext.Session.Remove("email");
            HttpContext.Session.Remove("user_id");
            return Redirect("/Home/Home");
        }


        [HttpPost]
        public string addtocart([FromBody] JsonDocument doc)
        {
            int product_id = doc.RootElement.GetProperty("product_id").GetInt32();
            int quantity = doc.RootElement.GetProperty("quantity").GetInt32(); 
            Console.WriteLine(product_id + " hello " +  quantity);
            var cartItem = db.Carts.FirstOrDefault(c => c.UserId == HttpContext.Session.GetInt32("user_id") && c.ProductId == product_id);
            if (cartItem != null)
            {
                cartItem.Quantity = cartItem.Quantity +1;
                db.SaveChanges();
                return JsonConvert.SerializeObject(new { flag = true });
            }
            else
            {
            db.Carts.Add(new Cart { UserId = HttpContext.Session.GetInt32("user_id"), ProductId = product_id, Quantity = quantity });  
            if(db.SaveChanges() == 1)
            return JsonConvert.SerializeObject(new { flag = true });
            else
                return JsonConvert.SerializeObject(new {flag = false});
            }            

        }

        [HttpGet]
        public string orders()
        {
            var orders = db.Orders
                .Join(db.Products,
                    o => o.ProductId,
                    p => p.ProductId,
                    (o, p) => new { Order = o, Product = p })
                .Where(op => op.Order.UserId == HttpContext.Session.GetInt32("user_id"))
                .OrderByDescending(op => op.Order.OrderTime)
                .Select(op => new {
                    op.Order.OrderId,
                    op.Order.OrderTime,
                    op.Order.Status,
                    op.Order.Quantity,
                    op.Product.ProductName,
                    op.Product.ProductPic,
                    op.Order.ItemValue,
                    op.Order.CustomerAddress
                })
                .ToList();
            return JsonConvert.SerializeObject(orders);
        }

        [HttpGet]
        public IActionResult MyOrders()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("email")))
            {
                return View("Error");
            }
            else
                return View();
        }





    }
}
