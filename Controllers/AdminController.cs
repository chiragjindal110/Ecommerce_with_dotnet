using Ecommerce.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text.Json;

namespace Ecommerce.Controllers
{
    public class AdminController : Controller
    {
        private readonly EcommerceContext db;

        public AdminController(EcommerceContext _db)
        {
            db = _db;
        }
        public IActionResult Index(string id)
        {
            if(id == "ch37a@892h$hh1123")
            {
                HttpContext.Session.SetString("admin-status", "1");
            return View();
            }
            else
                return View("Error");
        }

        [HttpGet]
        public IActionResult verifyadmin([FromQuery(Name = "password")] string password) 
        {
            Console.WriteLine("Hello  "+password);
            if(password == "chirag24@09")
            {
                Console.WriteLine(HttpContext.Session.GetString("admin-status"));
                if(!string.IsNullOrEmpty(HttpContext.Session.GetString("admin-status")) && HttpContext.Session.GetString("admin-status") == "1")
                {
                    return View("admin_page");
                }
            }

            return View("Error");   
        
        }

        [HttpPost]
        public string authorizeseller([FromBody] JsonDocument doc)
        {
            var seller_id = doc.RootElement.GetProperty("seller_id").GetInt32();
            var seller = db.Sellers.FirstOrDefault(s => s.SellerId == seller_id);
            if (seller != null)
            {
                seller.IsAuthorized = "true";
                if(db.SaveChanges()==1)
                {
                    return JsonConvert.SerializeObject(new {status = true});
                }
            }
            return JsonConvert.SerializeObject(new { status = false });
        }
    }
}
