using Ecommerce.Models;
using Mailjet.Client.Resources;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing.Constraints;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Collections.Immutable;
using System.Text.Json;

namespace Ecommerce.Controllers
{
    public class SellerController : Controller
    {
        private readonly EcommerceContext db;

        public SellerController(EcommerceContext _db)
        {
            db = _db;
        }
        public IActionResult Index()
        {

            if (string.IsNullOrEmpty(HttpContext.Session.GetString("email")))
            {
                return View("Error");
            }
            else if(HttpContext.Session.GetString("isVerified") == "false")
            {
                 return RedirectToAction("verifyMail", "Index");
            }
            else
            {
                var seller = db.Sellers.FirstOrDefault(s=>s.UserId == HttpContext.Session.GetInt32("user_id"));
                if(seller == null)
                {
                    return View("SellerForm");
                }
                if(seller!=null && seller.IsAuthorized == "false" && seller.Gst!="")
                {
                    return View("SellerRequestPage");
                }
                else if(seller != null && seller.IsAuthorized == "false")
                {
                    return View("SellerForm");
                }
                else if(seller!=null)
                {
                    return View("SellerDashboard");
                }
                else
                {
                    return View("Error");
                }
            }
        }

        [HttpGet]
        public string getsellerproducts() 
        {
            var products = db.Products
             .Join(db.Sellers,
                 p => p.SellerId,
                 s => s.SellerId,
                    (p, s) => new { Product = p, Seller = s })
             .Where(ps => ps.Seller.UserId == HttpContext.Session.GetInt32("user_id") && ps.Product.Status == 1)
             .Select(ps => new Product
             {
                 ProductId = ps.Product.ProductId,
                 ProductName = ps.Product.ProductName,
                 ProductPic = ps.Product.ProductPic,
                 ProductDescription = ps.Product.ProductDescription,
                 Stock = ps.Product.Stock,
                 Price = ps.Product.Price
             })
             .ToList();

            return JsonConvert.SerializeObject(products);
        }

        [HttpPost]
        public string removeproduct([FromBody]JsonDocument doc)
        {
            var pic = doc.RootElement.GetProperty("product_pic").GetString();   
            var obj = db.Products.FirstOrDefault(p=> p.ProductPic == pic);
            if (obj != null)
            {
                db.Products.Remove(obj);
                db.SaveChanges();
                return JsonConvert.SerializeObject(new {status = true});
            }
            else
            {
                return JsonConvert.SerializeObject(new { status = false});
            }
        }

        [HttpGet]
        public IActionResult orderscompleted()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("email")) || HttpContext.Session.GetString("isVerified")=="false")
            {
                return View("Error");
            }
            else
            {
                return View("SellerOrders");
            }
        }

        [HttpGet]
        public string getsellerorders()
        {

            var orders = db.Orders
                .Join(db.Products,
                    o => o.ProductId,
                    p => p.ProductId,
                    (o, p) => new { Order = o, Product = p })
                .Join(db.Sellers,
                    op => op.Product.SellerId,
                    s => s.SellerId,
                    (op, s) => new { op.Order, op.Product, Seller = s })
                .Where(ops => ops.Seller.UserId == HttpContext.Session.GetInt32("user_id"))
                .OrderByDescending(ops => ops.Order.OrderTime)
                .Select(ops => new {
                    ops.Product.ProductName,
                    ops.Product.ProductPic,
                    ops.Order.OrderId,
                    ops.Order.OrderTime,
                    ops.Order.Status,
                    ops.Order.Quantity,
                    ops.Order.ItemValue,
                    ops.Order.CustomerAddress
                })
                .ToList();
            return JsonConvert.SerializeObject(orders);
        }


        [HttpPost]
        public string addproduct(IFormCollection form, IFormFile picname)
        { 
            // Process form data and uploaded image
            string product_name = form["product_name"];
            string product_description = form["product_description"];
            int stock = Convert.ToInt32(form["product_stock"]);
            int price = Convert.ToInt32(form["product_price"]);
            string fileName = picname.FileName;

            // Define the file path where you want to save the image
            string filePath = Path.Combine("D:\\Visual Studio Projects\\Ecommerce\\wwwroot\\Product_Images\\", fileName);

            // Save the image to the file system
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                picname.CopyTo(stream);
            }
           var seller =  db.Sellers.Where(s => s.UserId == HttpContext.Session.GetInt32("user_id")).Select(s => s.SellerId).ToList();
            db.Products.Add(new Product { Price = price, ProductDescription = product_description, ProductName = product_name, ProductPic = fileName, Stock = stock, SellerId = seller[0] });
            if(db.SaveChanges()==1)
            {
                return JsonConvert.SerializeObject(new { status = true });
            }
            else
            {
                return JsonConvert.SerializeObject(new {status = false});
            }

            
        }

        [HttpPost]
        public string updateproduct(IFormCollection form, IFormFile picname)
        {
            var seller = db.Sellers.Where(s => s.UserId == HttpContext.Session.GetInt32("user_id")).Select(s => s.SellerId).ToList();
            string product_name = form["product_name"];
            string product_description = form["product_description"];
            int stock = Convert.ToInt32(form["product_stock"]);
            int price = Convert.ToInt32(form["product_price"]);
            int product_id = Convert.ToInt32(form["product_id"]);
            
            if (picname != null)
            {
                string fileName = picname.FileName;
                string filePath = Path.Combine("D:\\Visual Studio Projects\\Ecommerce\\wwwroot\\Product_Images\\", fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    picname.CopyTo(stream);
                }
            }
            var obj = db.Products.FirstOrDefault(p => p.ProductId == product_id);
            if (obj != null)
            {
                obj.Price = price;
                obj.Stock = stock;
                obj.ProductName = product_name;
                obj.ProductDescription = product_description;
                obj.ProductPic = obj.ProductPic;
                if (picname != null)
                {
                    obj.ProductPic = picname.FileName;
                }
                db.SaveChanges();
                return JsonConvert.SerializeObject(new { status = true });
            }
            return JsonConvert.SerializeObject(new { status = false });

        }

        [HttpPost]
        public string addseller([FromBody] JsonDocument doc)
        {
            var user_id = HttpContext.Session.GetInt32("user_id");
            if (user_id.HasValue)
            {
            var name = doc.RootElement.GetProperty("name").GetString();
            var gst = doc.RootElement.GetProperty("gst").GetString();
            var location = doc.RootElement.GetProperty("location").GetString();
            var seller = db.Sellers.FirstOrDefault(s => s.UserId == user_id);
                if (seller != null)
                {
                    seller.Address = location;
                    seller.SellerCompany = name;
                    seller.Gst = gst;
                }
                else
                {
                    db.Sellers.Add(new Seller { Address = location, Gst = gst, SellerCompany = name, IsAuthorized = "false", UserId = (int)user_id });
                }
                if(db.SaveChanges() ==1)
                {
                    return JsonConvert.SerializeObject(new {status = true});
                }
                return JsonConvert.SerializeObject(new { status = false });

            }
            return JsonConvert.SerializeObject(new { status = false });
        }

        [HttpGet]
        public IActionResult SellerRequestPage()
        {
            if (HttpContext.Session.GetString("isVerified") != null && HttpContext.Session.GetString("isVerified") == "false")
                return RedirectToAction("verifyMail", "Index");
            else
                return View();
        }

        [HttpGet]
        public string unauthorizedsellers()
        {
            var output = db.Sellers.Where(s => s.IsAuthorized == "false").ToList();
            return JsonConvert.SerializeObject(output);
        }
    }
}
