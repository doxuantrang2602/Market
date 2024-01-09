using Market.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Xml.Schema;

namespace Market.Controllers
{
    public class OdersController : Controller
    {
        private MarketContext db;
        private decimal Total = 0 ;
        public OdersController(MarketContext context)
        {
            db = context;
        }

        public string idOrder()
        {
            string idStart = "HD" + DateTime.Now.Day.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Year.ToString();
            int id = 0;
            bool check = false;
            string idOders = "";
            while (check == false)
            {
                var item = db.Orders.Find(idStart + id.ToString());
                if (item == null)
                {
                    check = true;
                }
                else
                {
                    id++;
                    check = false;
                }
            }
            idOders = idStart + id.ToString();
            return idOders;
        }

        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public IActionResult Checkout(decimal total,string id)
        {
            ViewBag.Total = total;
            ViewBag.OrderId = id;
            return View();
        }

        public IActionResult Update(decimal total)
        {
            return RedirectToAction("Checkout", new { total = total ,id = idOrder() });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Insert([Bind("OrderId,CustomerName,Address,Phone,Note,TotalOrders")] Order oders)
        {
            oders.OrderDate = DateTime.Now.Date;
            db.Orders.Add(oders);
            db.SaveChanges();
            List<ShoppingCart> item = db.ShoppingCarts.Where(m => m.ShoppingCartId == "001").ToList();
            foreach (ShoppingCart cart in item)
            {
                OrderDetail detail = new OrderDetail();
                detail.OrderId = oders.OrderId;
                detail.ProductId = cart.ProductId;
                detail.Price = cart.Price;
                detail.Amount = cart.Amount;
                detail.Sale = 0;
                detail.Total = cart.Total;
                db.OrderDetails.Add(detail);
            }
            db.SaveChanges();
            foreach (ShoppingCart cart in item)
            {
                db.ShoppingCarts.Remove(cart);
            }
            db.SaveChanges();
            return RedirectToAction("Checkout");
        }
    }
}
