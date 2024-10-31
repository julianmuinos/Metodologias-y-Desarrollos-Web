using Antlr.Runtime;
using MyApp.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;
using static MyApp.Core.Models.Context;

namespace MyApp.Core.Controllers
{
    public class CartController : Controller
    {
        private static List<Product> products = MyApp.Core.Models.Context.Products.ListAll();
        private static User user = MyApp.Core.Models.Context.Users.ListAll().FirstOrDefault(x => x.Id == MyApp.Services.Session.GetInstance.id);
        private Cart cart;
        User pUser;
        CartService cartService = new CartService();

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            cartService.InitializeCart(user.Id);
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
                ViewBag.Items = cartService.GetCart().GetItems();
                return View(cartService.GetCart());
            }

            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult SalesHistory()
        {
            if (MyApp.Services.Session.GetInstance == null) return RedirectToAction("Login", "Users");
            InitializeUser();

            if (pUser.Role == "webmaster")
            {
                string rutaXML = Server.MapPath("~/Cart.xml");
                XDocument xmlDocumento = XDocument.Load(rutaXML);
                return View(xmlDocumento);
            }

            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult AddItem(int productId, uint quantity = 1)
        {
            cartService.AddItem(productId, quantity);
            HttpContext.Session.Add("Cart", cartService.GetCart());
            return RedirectToAction("Index");
        }

        public ActionResult SubtractItem(int productId, uint quantity = 1)
        {
            cartService.SubtractItem(productId, quantity);
            return RedirectToAction("Index");
        }

        public ActionResult RemoveItem(int productId)
        {
            cartService.RemoveItem(productId);
            return RedirectToAction("Index");
        }

        public ActionResult ConfirmPurchase()
        {
            cartService.ConfirmPurchase();
            return RedirectToAction("Index");
        }
    }
}