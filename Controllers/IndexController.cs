
using Ecommerce.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Text.Json;


using Mailjet.Client;
using Mailjet.Client.Resources;
using System.Threading.Tasks;
using Mailjet.Client.TransactionalEmails;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;

namespace Ecommerce.Controllers
{
    public class IndexController : Controller
    {
        private readonly ILogger<IndexController> _logger;
        

        public IndexController(ILogger<IndexController> logger)
        {
            _logger = logger;
        }
      

        public IActionResult Index()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("email")))
            {
                return View();
            }
            else
            {
                return Redirect("Home/Home");
            }
            
        }

            EcommerceContext db = new EcommerceContext();
        [HttpPost]
        public string authenticate([FromBody]JsonDocument doc)
        {
            var email = doc.RootElement.GetProperty("email").GetString();
            var password = doc.RootElement.GetProperty("password").GetString();
            bool valid_email = Regex.IsMatch(email, @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", RegexOptions.IgnoreCase);
            
            var output = db.Users.Where(p=>p.Email == email && p.Password == password).ToList();
            if(!valid_email) { return JsonConvert.SerializeObject(new { flag = false }); }
            if(output.Count ==1)
            {
                HttpContext.Session.SetString("email", email);
                HttpContext.Session.SetInt32("user_id", output[0].UserId);
                HttpContext.Session.SetString("address", output[0].Address);
                HttpContext.Session.SetInt32("token", (int)output[0].Token);
                HttpContext.Session.SetString("isVerified", output[0].Verified);
                return JsonConvert.SerializeObject(new { flag = true });
            }
            else
            {
                return JsonConvert.SerializeObject(new { flag = false });
            }
        }

        [HttpPost]
        public string validate([FromBody] JsonDocument doc)
        {
            try
            {
                var email = doc.RootElement.GetProperty("email").GetString();
                var password = doc.RootElement.GetProperty("password").GetString();
                bool valid_email = Regex.IsMatch(email, @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", RegexOptions.IgnoreCase);
                bool valid_password = Regex.IsMatch(password, "^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{8,}$", RegexOptions.IgnoreCase);
                var phone = doc.RootElement.GetProperty("phone").GetInt64();
                bool valid_phone = Regex.IsMatch(phone + "", "^\\+?[1-9][0-9]{7,14}$", RegexOptions.IgnoreCase);
                if (!valid_email || !valid_password || !valid_phone)
                {
                    throw new Exception();
                }
                if (db.Users.Where(p => p.Email == email).ToList().Count == 1)
                    throw new Exception();

                var name = doc.RootElement.GetProperty("name").GetString();
                Random random = new Random();
            var token = random.Next(2000, 100000);
            db.Users.Add(new Models.User() { Email = email, Name = name, Password = password, Phone = (long)phone,Token = token });
            HttpContext.Session.SetString("email", email);
            HttpContext.Session.SetInt32("user_id", -1);
                HttpContext.Session.SetString("address", "");
                HttpContext.Session.SetString("isVerified", "false");
                SendMail(email,name,token,0).Wait();
            db.SaveChanges();
               return JsonConvert.SerializeObject(new { flag = true });
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return JsonConvert.SerializeObject(new { flag = false });
            }
        }

        public async Task SendMail( string email,string name,long token,int mode)
        {
            MailjetClient client = new MailjetClient("ed4864c1bfaa9f4def636d4e34b6ef9d", "d283715c82e7059aeaef312065bbd351");
         
            Console.WriteLine(email + "," + name + "," + token);
            MailjetRequest request = new MailjetRequest
            {
                Resource = SendV31.Resource,
            };
            string url = "";
            string html = "";
            string subject = "";
            if(mode == 0)
            {
                subject = "Verify Your Email at Healthifyy";
                url = "https://localhost:44329/Index/verifymailHealthifyy?token=" + token;
                html = "<h3>Dear "+name +" , welcome to Healthifyy!</h3><br />Verify Mail by clicking <a href=" + url + ">Verify</a>";
            }
            else
            {
                subject = "Healthify: Change Password";
                url = "https://localhost:44329/Index/changePassword/" + token;
                html = "<h3>Dear"+name +" , welcome to Healthifyy!</h3><br />Change Password by clicking <a href=" + url + ">ChangePassword</a>";
            }
              request.Property(Send.Messages, new JArray {
                new JObject {
                 {"From", new JObject {
                  {"Email", "akshita1219209@jmit.ac.in"},
                  {"Name", "Healthifyy"},
                  }},
                 {"To", new JArray {
                  new JObject {
                   {"Email", email}
                   }
                  }},
                 {"Subject", subject},
                 {"TextPart", "Greetings from Mailjet!"},
                 {"HTMLPart", html}
                 }
                   });
            MailjetResponse response = await client.PostAsync(request);
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine(string.Format("Total: {0}, Count: {1}\n", response.GetTotal(), response.GetCount()));
                Console.WriteLine(response.GetData());
                
            }
            else
            {
                Console.WriteLine(string.Format("StatusCode: {0}\n", response.StatusCode));
                Console.WriteLine(string.Format("ErrorInfo: {0}\n", response.GetErrorInfo()));
                Console.WriteLine(response.GetData());
                Console.WriteLine(string.Format("ErrorMessage: {0}\n", response.GetErrorMessage()));
                
            }
        }

        [HttpPost]
        public string forgotPassword([FromBody] JsonDocument doc)
        {
            var mail = doc.RootElement.GetProperty("email").GetString();
            var user = db.Users.FirstOrDefault(u => u.Email == mail);
            if(user == null)return JsonConvert.SerializeObject(new {status = false});
            Console.WriteLine(user.Token);
            SendMail(mail, user.Name, user.Token, 1).Wait();
            return JsonConvert.SerializeObject(new {status = true});
        }

        [HttpGet]
        public IActionResult changePassword(long id)
        {
            return View(id);
        }
        [HttpPost]
        public string changePasswordValue([FromBody] JsonDocument doc)
        {
            var password = doc.RootElement.GetProperty("password").GetString();
            var token = doc.RootElement.GetProperty("token").GetInt64();
            Console.WriteLine(token);
            var user = db.Users.FirstOrDefault(u => u.Token == token);
            Console.WriteLine(user);
            if(user!= null)
            {
                user.Password = password;
                HttpContext.Session.SetString("email", user.Email);
                HttpContext.Session.SetInt32("user_id", user.UserId);
                HttpContext.Session.SetString("address", user.Address);
                HttpContext.Session.SetInt32("token", (int)token);
                HttpContext.Session.SetString("isVerified", user.Verified);
                db.SaveChanges();
                return JsonConvert.SerializeObject(new { status = true });
            }
            return JsonConvert.SerializeObject(new { status = false });
        }

        [HttpGet]
        public IActionResult verifyMail() 
        {
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("email")))
            {
                return View();
            }
            else
                return View("Error");
        
        }
        [HttpGet]
        public IActionResult verifymailHealthifyy([FromQuery(Name = "token")] long token)
        {
            Console.WriteLine(token);
            var user = db.Users.FirstOrDefault(u => u.Token == token);
            if (user != null)
            {
                user.Verified = "true";
                HttpContext.Session.SetString("isVerified", "true");
                HttpContext.Session.SetString("email", user.Email);
                HttpContext.Session.SetInt32("user_id", user.UserId);
                HttpContext.Session.SetString("address", user.Address);
                HttpContext.Session.SetInt32("token", (int)token);
                db.SaveChanges();
                return RedirectToAction("Home", "Home");
            }
            else
                return null;
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}