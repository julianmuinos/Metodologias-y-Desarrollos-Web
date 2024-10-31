using Antlr.Runtime;
using MyApp.Core.Models;
using MyApp.Interfaces;
using MyApp.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static MyApp.Core.Models.Context;

namespace MyApp.Core.Controllers
{
    public class ProductsController : Controller
    {
        User pUser;

        private void InitializeUser()
        {
            if (MyApp.Services.Session.GetInstance != null)
            {
                pUser = Context.Users.ListAll().FirstOrDefault(x => x.Id == MyApp.Services.Session.GetInstance.id);
            }
        }

        // GET: Products
        public ActionResult Index()
        {
            if (MyApp.Services.Session.GetInstance == null) return RedirectToAction("Login", "Users");
            InitializeUser();

            if (pUser.Role == "webmaster" || pUser.Role == "admin")
            {
                List<Product> product = Context.Products.ListAll();
                ViewBag.Products = product;
                return View();
            }

            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        // GET: CardsProducts
        public ActionResult CardsProducts()
        {
            if (MyApp.Services.Session.GetInstance == null) return RedirectToAction("Login", "Users");
            InitializeUser();

            if (pUser.Role == "webmaster" || pUser.Role == "admin" || pUser.Role == "user")
            {
                List<Product> product = Context.Products.ListAll();
                ViewBag.CardProducts = product;
                return View();
            }

            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        // GET: Products/Create
        public ActionResult Create()
        {
            if(MyApp.Services.Session.GetInstance == null) return RedirectToAction("Login", "Users");
            InitializeUser();

            if (pUser.Role == "webmaster" || pUser.Role == "admin")
            {
                List<Product> product = Context.Products.ListAll();
                ViewBag.CardProducts = product;
                return View();
            }

            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        // POST: Products/Create
        [HttpPost]
        public ActionResult Create(Product pProduct, HttpPostedFileBase pImagenArchivo)
        {
            try
            {
                if (MyApp.Services.Session.GetInstance == null) return RedirectToAction("Login", "Users");
                InitializeUser();
                if (pImagenArchivo != null && pImagenArchivo.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(pImagenArchivo.FileName);
                    var path = Path.Combine(Server.MapPath("~/Images/"), fileName);
                    pImagenArchivo.SaveAs(path);
                    pProduct.ImageURL = "/Images/" + fileName;
                }
                Context.Products.Add(pProduct);
                Verificator.SetVerificator(Context.Products.ListAll().ToList<IEntity>(), "Product");
                MyApp.Services.Logger.AddLog("Add product", 2, pUser.Email);

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Products/Edit/5
        public ActionResult Edit(int id)
        {
            if (MyApp.Services.Session.GetInstance == null) return RedirectToAction("Login", "Users");
            InitializeUser();

            if (pUser.Role == "webmaster" || pUser.Role == "admin")
            {
                var product = Context.Products.ListAll().FirstOrDefault(p => p.Id == id);
                if (product == null)
                {
                    return HttpNotFound();
                }
                return View(product);
            }

            else
            {
                return RedirectToAction("Index", "Home");
            }            
        }

        // POST: Products/Edit/5
        [HttpPost]
        public ActionResult Edit(Product pProduct, HttpPostedFileBase pImagenArchivo)
        {
            try
            {
                if (MyApp.Services.Session.GetInstance == null) return RedirectToAction("Login", "Users");
                InitializeUser();
                if (ModelState.IsValid)
                {
                    var producto = Context.Products.ListAll().FirstOrDefault(p => p.Id == pProduct.Id);
                    if (pImagenArchivo != null && pImagenArchivo.ContentLength > 0)
                    {
                        var fileName = Path.GetFileName(pImagenArchivo.FileName);
                        var path = Path.Combine(Server.MapPath("~/Images/"), fileName);
                        pImagenArchivo.SaveAs(path);
                        pProduct.ImageURL = "/Images/" + fileName;
                    }
                    else
                    {
                        pProduct.ImageURL = producto.ImageURL;
                    }
                    Context.Products.Modify(pProduct);
                    Verificator.SetVerificator(Context.Products.ListAll().ToList<IEntity>(), "Product");
                    MyApp.Services.Logger.AddLog("Modify product", 2, pUser.Email);
                    return RedirectToAction("Index");
                }
                return View(pProduct);
            }
            catch
            {
                return View(pProduct);
            }
        }

        // GET: Products/Delete/5
        public ActionResult Delete(int id)
        {
            if (MyApp.Services.Session.GetInstance == null) return RedirectToAction("Login", "Users");
            InitializeUser();

            if (pUser.Role == "webmaster" || pUser.Role == "admin")
            {
                var product = Context.Products.ListAll().FirstOrDefault(p => p.Id == id);
                if (product == null)
                {
                    return HttpNotFound();
                }
                return View(product);
            }

            else
            {
                return RedirectToAction("Index", "Home");
            }            
        }

        // POST: Products/Delete/5
        [HttpPost]
        public ActionResult Delete(Product pProduct)
        {
            try
            {
                if (MyApp.Services.Session.GetInstance == null) return RedirectToAction("Login", "Users");
                InitializeUser();
                Context.Products.Delete(pProduct.Id);
                Verificator.SetVerificator(Context.Products.ListAll().ToList<IEntity>(), "Product");
                MyApp.Services.Logger.AddLog("Delete product", 1, pUser.Email);
                return RedirectToAction("Index");
            }
            catch
            {
                return View(pProduct);
            }
        }
    }
}
