using MyApp.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyApp.Core.Controllers
{
    public class HomeController : Controller
    {
        User pUser;

        private void InitializeUser()
        {
            if (MyApp.Services.Session.GetInstance != null)
            {
                pUser = Context.Users.ListAll().FirstOrDefault(x => x.Id == MyApp.Services.Session.GetInstance.id);
            }
        }

        public ActionResult Index()
        {
            if (MyApp.Services.Session.GetInstance != null)
            {
                if (MyApp.Services.Verificator.CompareVerificator(MyApp.Core.Models.Context.Users.ListAll().ToList<MyApp.Interfaces.IEntity>(), "User"))
                {
                    if (MyApp.Services.Verificator.CompareVerificator(MyApp.Core.Models.Context.Products.ListAll().ToList<MyApp.Interfaces.IEntity>(), "Product"))
                    {
                        return View();
                    }
                }
                return RedirectToAction("BackupRestore");
            }

            else
            {
                return View();
            }
        }

        public ActionResult Log(DateTime? startDate, DateTime? endDate)
        {
            if (MyApp.Services.Session.GetInstance == null) return RedirectToAction("Login", "Users");
            InitializeUser();

            if (pUser.Role == "webmaster" || pUser.Role == "admin")
            {
                if (startDate == null)
                {
                    startDate = DateTime.Now.AddDays(-30);
                }
                if (endDate == null)
                {
                    endDate = DateTime.Now;
                }

                var logs = MyApp.Services.Logger.GetLogs(startDate.Value, endDate.Value);
                ViewBag.Logs = logs;
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult BackupRestore()
        {
            if (MyApp.Services.Session.GetInstance == null) return RedirectToAction("Login", "Users");
            InitializeUser();

            if (pUser.Role == "webmaster")
            {
                return View();
            }

            else
            {
                return RedirectToAction("MessageError", "Home");
            }
        }

        public ActionResult MessageError()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Privacy()
        {
            return View();
        }
    }
}