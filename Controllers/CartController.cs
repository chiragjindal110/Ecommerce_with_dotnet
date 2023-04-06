using Ecommerce.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text.Json;

namespace Ecommerce.Controllers
{
    public class CartController : Controller
    {


        private readonly EcommerceContext db;

        public CartController(EcommerceContext _db)
        {
            db = _db;
        }
        [HttpGet]
        public IActionResult cart()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("email")))
            {
                return View(false);
            }
            else
            {
                return View(true);
            }
        }

        [HttpGet]
        public string getcart()
        {

            var result = db.Carts
                   .Join(db.Products, c => c.ProductId, p => p.ProductId, (c, p) => new { Cart = c, Product = p })
                   .Where(cp => cp.Cart.UserId == HttpContext.Session.GetInt32("user_id") && cp.Product.Status == 1)
                   .Select(cp => new {
                       cp.Product.Stock,
                       cp.Product.ProductId,
                       cp.Cart.Quantity,
                       cp.Product.ProductName,
                       cp.Product.ProductPic,
                       cp.Product.ProductDescription,
                       cp.Product.Price
                   });


            var json = JsonConvert.SerializeObject(result);
            return json;

        }

        [HttpPost]
        public string cartquantitychange([FromBody] JsonDocument doc)
        {
            int product_id = doc.RootElement.GetProperty("product_id").GetInt32();
            int change = doc.RootElement.GetProperty("change").GetInt32();
            var item = db.Carts.FirstOrDefault(c=> c.UserId == HttpContext.Session.GetInt32("user_id") &&  c.ProductId == product_id);
            if(item!=null)item.Quantity = item.Quantity + change;
            if (db.SaveChanges() == 1)
            {
                return JsonConvert.SerializeObject(new { status = true });
            }
            return JsonConvert.SerializeObject(new { status = false });

        }

        [HttpPost]
        public string removeproductfromcart([FromBody] JsonDocument doc)
        {
            int product_id = doc.RootElement.GetProperty("product_id").GetInt32();
            var cartItem = db.Carts.FirstOrDefault(c => c.UserId == HttpContext.Session.GetInt32("user_id") && c.ProductId == product_id);
            if (cartItem != null)
            {
                db.Carts.Remove(cartItem);
            }
            if (db.SaveChanges() == 1)
            {
                return JsonConvert.SerializeObject(new { status = true });
            }
            return JsonConvert.SerializeObject(new { status = false });
        }

    }
}
