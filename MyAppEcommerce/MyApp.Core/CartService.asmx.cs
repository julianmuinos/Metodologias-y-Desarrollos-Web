using MyApp.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using static MyApp.Core.Models.Context;

namespace MyApp.Core
{
    /// <summary>
    /// Descripción breve de CartService
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // Para permitir que se llame a este servicio web desde un script, usando ASP.NET AJAX, quite la marca de comentario de la línea siguiente. 
    //[System.Web.Script.Services.ScriptService]
    public class CartService : System.Web.Services.WebService
    {
        private Cart cart;

        [WebMethod(EnableSession = true)]
        public Cart GetCart()
        {
            Cart cart = HttpContext.Current.Session["Cart"] as Cart;
            if (cart == null)
            {
                cart = new Cart();
                HttpContext.Current.Session["Cart"] = cart;
            }
            return cart;
        }

        [WebMethod(EnableSession = true)]
        public void InitializeCart(int userId)
        {
            cart = HttpContext.Current.Session["Cart"] as Cart;

            if (cart == null)
            {
                User user = Users.ListAll().FirstOrDefault(x => x.Id == userId);
                cart = new Cart(user);
                HttpContext.Current.Session["Cart"] = cart;
            }
        }

        [WebMethod(EnableSession = true)]
        public void AddItem(int productId, uint quantity = 1)
        {
            cart = HttpContext.Current.Session["Cart"] as Cart;
            var product = Products.ListAll().FirstOrDefault(p => p.Id == productId);
            if (product != null)
            {
                cart.AddItem(product, quantity);
            }
            HttpContext.Current.Session["Cart"] = cart;
        }

        [WebMethod(EnableSession = true)]
        public void SubtractItem(int productId, uint quantity = 1)
        {
            cart = HttpContext.Current.Session["Cart"] as Cart;
            var product = Products.ListAll().FirstOrDefault(p => p.Id == productId);
            if (product != null)
            {
                cart.SubtractItem(product, quantity);
            }
            HttpContext.Current.Session["Cart"] = cart;
        }

        [WebMethod(EnableSession = true)]
        public void RemoveItem(int productId)
        {
            cart = HttpContext.Current.Session["Cart"] as Cart;
            var product = Products.ListAll().FirstOrDefault(p => p.Id == productId);
            if (product != null)
            {
                cart.SubtractItem(product, cart.GetItems().FirstOrDefault(i => i.Product.Id == productId).Quantity);
            }

            HttpContext.Current.Session["Cart"] = cart;
        }

        [WebMethod(EnableSession = true)]
        public decimal GetTotal()
        {
            cart = HttpContext.Current.Session["Cart"] as Cart;
            return cart.Total;
        }

        [WebMethod(EnableSession = true)]
        public void ConfirmPurchase()
        {
            cart = HttpContext.Current.Session["Cart"] as Cart;
            Carts.Create(cart);
        }
    }
}
