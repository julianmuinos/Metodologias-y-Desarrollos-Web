using Antlr.Runtime;
using MyApp.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static MyApp.Core.Models.Context;

namespace MyApp.Core.Controllers
{
    public class CartController : Controller
    {
        private static List<Product> products = MyApp.Core.Models.Context.Products.ListAll();
        private static User user = MyApp.Core.Models.Context.Users.ListAll().FirstOrDefault(x => x.Id == MyApp.Services.Session.GetInstance.id);
        private Cart cart;
        User pUser;

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            cart = (HttpContext.Session["Cart"] as Cart) == null ? new Cart(user) : HttpContext.Session["Cart"] as Cart;
        }

        private void InitializeUser()
        {
            if (MyApp.Services.Session.GetInstance != null)
            {
                pUser = Context.Users.ListAll().FirstOrDefault(x => x.Id == MyApp.Services.Session.GetInstance.id);
            }
        }

        public ActionResult Index()
        {
            if (MyApp.Services.Session.GetInstance == null) return RedirectToAction("Login", "Users");
            InitializeUser();

            if (pUser.Role == "webmaster" || pUser.Role == "user")
            {
                ViewBag.Products = products;
                ViewBag.Items = cart.GetItems();
                return View(cart);
            }

            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult AddItem(int productId, uint quantity = 1)
        {
            var product = products.FirstOrDefault(p => p.Id == productId);

            if (product != null)
            {
                cart.AddItem(product, quantity);
            }

            HttpContext.Session.Add("Cart", cart);
            return RedirectToAction("Index");
        }

        public ActionResult SubtractItem(int productId, uint quantity = 1)
        {
            var product = products.FirstOrDefault(p => p.Id == productId);

            if (product != null)
            {
                cart.SubtractItem(product, quantity);
            }

            return RedirectToAction("Index");
        }

        public ActionResult RemoveItem(int productId)
        {
            var product = products.FirstOrDefault(p => p.Id == productId);

            if (product != null)
            {
                cart.SubtractItem(product, cart.GetItems().FirstOrDefault(i => i.Product.Id == productId).Quantity);
            }

            return RedirectToAction("Index");
        }

        public ActionResult ConfirmPurchase()
        {
            Carts.Create(cart);
            return RedirectToAction("Index");
        }
    }
}