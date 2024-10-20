using MyApp.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using static MyApp.Core.Models.Context;
using System.Xml;
using System.Xml.Xsl;
using System.IO;

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
            SaveCartToXml();
            HttpContext.Current.Session["Cart"] = null;
            InitializeCart(Users.ListAll().FirstOrDefault(x => x.Id == Services.Session.GetInstance.id).Id);
        }

        private void SaveCartToXml()
        {
            string filePath = Server.MapPath("Cart.xml");

            XmlDocument doc = new XmlDocument();

            if (File.Exists(filePath) && new FileInfo(filePath).Length > 0)
            {
                try
                {
                    doc.Load(filePath);
                }
                catch (XmlException)
                {
                    doc = new XmlDocument();
                    XmlDeclaration xmlDecl = doc.CreateXmlDeclaration("1.0", "utf-8", null);
                    doc.AppendChild(xmlDecl);
                    XmlNode root = doc.CreateElement("Carts");
                    doc.AppendChild(root);
                }

                if (doc.DocumentElement == null)
                {
                    XmlNode root = doc.CreateElement("Carts");
                    doc.AppendChild(root);
                }
            }
            else
            {
                XmlDeclaration xmlDecl = doc.CreateXmlDeclaration("1.0", "utf-8", null);
                doc.AppendChild(xmlDecl);
                XmlNode root = doc.CreateElement("Carts");
                doc.AppendChild(root);
            }

            XmlNode cartNode = doc.CreateElement("Cart");

            XmlNode userNode = doc.CreateElement("Usuario");

            XmlNode nameNode = doc.CreateElement("Nombre");
            nameNode.InnerText = Users.ListAll().FirstOrDefault(x => x.Id == MyApp.Services.Session.GetInstance.id).Username;
            userNode.AppendChild(nameNode);

            XmlNode emailNode = doc.CreateElement("Email");
            emailNode.InnerText = Users.ListAll().FirstOrDefault(x => x.Id == MyApp.Services.Session.GetInstance.id).Email;
            userNode.AppendChild(emailNode);

            cartNode.AppendChild(userNode);

            XmlNode dateNode = doc.CreateElement("Fecha");
            dateNode.InnerText = DateTime.Now.ToString();
            cartNode.AppendChild(dateNode);

            XmlNode itemsNode = doc.CreateElement("Items");

            foreach (Item item in cart.GetItems())
            {
                XmlNode itemNode = doc.CreateElement("Item");

                XmlNode productNode = doc.CreateElement("Producto");
                productNode.InnerText = item.Product.Name;
                itemNode.AppendChild(productNode);

                XmlNode priceNode = doc.CreateElement("Precio");
                priceNode.InnerText = item.Product.Price.ToString();
                itemNode.AppendChild(priceNode);

                XmlNode quantityNode = doc.CreateElement("Cantidad");
                quantityNode.InnerText = item.Quantity.ToString();
                itemNode.AppendChild(quantityNode);

                itemsNode.AppendChild(itemNode);
            }

            cartNode.AppendChild(itemsNode);

            doc.DocumentElement.AppendChild(cartNode);

            doc.Save(filePath);
        }
    }
}
