using Market.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Market.Controllers
{
    public class CartController : Controller
    {
        private MarketContext db;
        public CartController(MarketContext context)
        {
            db = context;
        }

        

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult ShopingCart()
        {
            var cart = db.ShoppingCarts.Where(m => m.ShoppingCartId == "001").ToList();
            ViewBag.CartId = "001";
            return View(cart);
        }
        public IActionResult AddShopingCart(String? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var product = db.Products.Where(m => m.ProductId == id).FirstOrDefault();
            if (product == null)
            {
                return NotFound();
            }
            else
            {
                var cartitem = db.ShoppingCarts.Where(m => m.ProductId == id && m.ShoppingCartId == "001").FirstOrDefault();
                if (cartitem != null)
                {
                    cartitem.Amount++;
                    cartitem.Total = cartitem.Amount * cartitem.Price;
                    db.ShoppingCarts.Update(cartitem);
                }
                else if(cartitem == null)
                {
                    ShoppingCart cart = new ShoppingCart
                    {
                        Img = product.Img,
                        ProductName = product.ProductName,
                        Price = product.Price,
                        Amount = 1,
                        Unit = product.Unit,
                        ProductId = product.ProductId,
                        Total = product.Price,
                        Status = 1,
                        ShoppingCartId = "001"
                    };
                    db.ShoppingCarts.Add(cart);
                }
                db.SaveChanges();
            }
            return RedirectToAction("ShopingCart");
        }

        [HttpPost]
        public IActionResult UpdateItem(String? id,[Bind("Amount")] ShoppingCart cart)
        {
            if(id == null || db.ShoppingCarts == null)
            {
                return NotFound();
            }
            if(ModelState.IsValid)
            {
                try
                {
                    db.ShoppingCarts.Update(cart);
                    db.SaveChanges();
                }
                catch(DbUpdateConcurrencyException)
                {
                    if(db.ShoppingCarts.Where(m => m.ProductId == id && m.ShoppingCartId == "001").FirstOrDefault() != null)
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("ShopingCart");
            }
            return RedirectToAction("ShopingCart");
        }


        public IActionResult DeleteItem(String? id)
        {
            if(id == null)
            {
                return NotFound();
            }
            else
            {
                var cartitem = db.ShoppingCarts.FirstOrDefault(m => m.ProductId == id);
                if (cartitem != null)
                {
                    db.ShoppingCarts.Remove(cartitem);
                }
                db.SaveChanges();
            }
            return RedirectToAction("ShopingCart");
        }
    }
}
