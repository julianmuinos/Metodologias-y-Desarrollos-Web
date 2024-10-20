using MyApp.Core.Models;
using MyApp.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyApp.Core.Controllers
{
    public class BackupRestoreController : Controller
    {
        User pUser;

        private void InitializeUser()
        {
            if (MyApp.Services.Session.GetInstance != null)
            {
                pUser = Context.Users.ListAll().FirstOrDefault(x => x.Id == MyApp.Services.Session.GetInstance.id);
            }
        }

        [HttpPost]
        public ActionResult Backup()
        {
            InitializeUser();
            string aux = MyApp.Services.Backup.CreateBackup(pUser.Email);
            TempData["Message"] = aux;

            return RedirectToAction("BackupRestore", "Home");
        }

        [HttpPost]
        public ActionResult Restore(HttpPostedFileBase pBackup)
        {
            try
            {
                InitializeUser();
                var path = "";
                if (pBackup != null && pBackup.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(pBackup.FileName);
                    path = Path.Combine(@"C:\Backup\", fileName);
                    string aux = MyApp.Services.Backup.RestoreBackup(pUser.Email, path);
                    TempData["Message"] = aux;
                }
            }
            catch (Exception ex)
            {
                TempData["Message"] = "Error al realizar el restore: " + ex.Message;
            }
            return RedirectToAction("BackupRestore", "Home");
        }

        [HttpPost]
        public ActionResult RecalculateDVProduct()
        {
            Services.Verificator.SetVerificator(Context.Products.ListAll().ToList<IEntity>(), "Product");
            return RedirectToAction("BackupRestore", "Home");
        }

        [HttpPost]
        public ActionResult RecalculateDVUser()
        {
            Services.Verificator.SetVerificator(Context.Users.ListAll().ToList<IEntity>(), "User");
            return RedirectToAction("BackupRestore", "Home");
        }
    }
}