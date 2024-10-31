using MyApp.Core.Models;
using MyApp.Interfaces;
using MyApp.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace MyApp.Core.Controllers
{
    public class UsersController : Controller
    {
        User pUser;

        private void InitializeUser()
        {
            if (MyApp.Services.Session.GetInstance != null)
            {
                pUser = Context.Users.ListAll().FirstOrDefault(x => x.Id == MyApp.Services.Session.GetInstance.id);
            }
        }

        // GET: Users
        public ActionResult Index()
        {
            if (MyApp.Services.Session.GetInstance == null) return RedirectToAction("Login", "Users");
            InitializeUser();

            if (pUser.Role == "webmaster" || pUser.Role == "admin")
            {
                List<User> user = Context.Users.ListAll();
                ViewBag.Users = user;
                return View();
            }

            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        // GET: Users/Create
        public ActionResult Create()
        {
            if (MyApp.Services.Session.GetInstance == null) return RedirectToAction("Login", "Users");
            InitializeUser();

            if (pUser.Role == "webmaster" || pUser.Role == "admin")
            {
                return View();
            }

            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        // POST: Users/Create
        [HttpPost]
        public ActionResult Create(User pUser)
        {
            try
            {
                if (MyApp.Services.Session.GetInstance == null) return RedirectToAction("Login", "Users");
                Context.Users.Add(pUser);
                Verificator.SetVerificator(Context.Users.ListAll().ToList<IEntity>(), "User");
                Logger.AddLog("User added", 2, pUser.Email);
                return RedirectToAction("Index", "Users");
            }
            catch
            {
                return View();
            }
        }

        // GET: Users/Edit/5
        public ActionResult Edit(int? id)
        {
            if (MyApp.Services.Session.GetInstance == null) return RedirectToAction("Login", "Users");
            InitializeUser();

            if (pUser.Role == "webmaster" || pUser.Role == "admin")
            {
                var user = Context.Users.ListAll().FirstOrDefault(p => p.Id == id);

                if (user == null)
                {
                    return HttpNotFound();
                }

                return View(user);
            }

            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        // POST: Users/Edit/5
        [HttpPost]
        public ActionResult Edit(User pUser)
        {
            try
            {
                if (MyApp.Services.Session.GetInstance == null) return RedirectToAction("Login", "Users");
                Context.Users.Modify(pUser);
                Verificator.SetVerificator(Context.Users.ListAll().ToList<IEntity>(), "User");
                Logger.AddLog("User modified", 2, pUser.Email);
                return RedirectToAction("Index", "Users");
            }
            catch
            {
                return View();
            }
        }

        // GET: Users/Delete/5
        public ActionResult Delete(int? id)
        {
            if (MyApp.Services.Session.GetInstance == null) return RedirectToAction("Login", "Users");
            InitializeUser();

            if (pUser.Role == "webmaster" || pUser.Role == "admin")
            {
                var user = Context.Users.ListAll().FirstOrDefault(p => p.Id == id);

                if (user == null)
                {
                    return HttpNotFound();
                }

                return View(user);
            }

            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        // POST: Users/Delete/5
        [HttpPost]
        public ActionResult Delete(User pUser)
        {
            try
            {
                if (MyApp.Services.Session.GetInstance == null) return RedirectToAction("Login", "Users");
                var user = Context.Users.ListAll().FirstOrDefault(u => u.Id == pUser.Id);
                Context.Users.Delete(user.Id);
                Verificator.SetVerificator(Context.Users.ListAll().ToList<IEntity>(), "User");
                Logger.AddLog("User deleted", 2, user.Email);
                return RedirectToAction("Index", "Users");
            }
            catch
            {
                return View();
            }
        }

        // POST: Users/Unblock/
        [HttpPost]
        public ActionResult Unblock(int id)
        {
            try
            {
                if (MyApp.Services.Session.GetInstance == null)
                    return RedirectToAction("Login", "Users");

                var user = Context.Users.ListAll().FirstOrDefault(u => u.Id == id);
                if (user != null)
                {
                    Context.Users.Unblock(user);
                    Verificator.SetVerificator(Context.Users.ListAll().ToList<IEntity>(), "User");
                    Logger.AddLog("User unblocked", 2, user.Email);
                }

                return RedirectToAction("Index", "Users");
            }
            catch
            {
                return View();
            }
        }

        // GET: Users/Login
        [AllowAnonymous]
        public ActionResult Login()
        {
            return View();
        }

        // POST: Users/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(User pUser)
        {
            if (Services.Session.GetInstance == null && pUser.Email != null && pUser.Password != null)
            {
                var user = Context.Users.ListAll().FirstOrDefault(u => u.Email == pUser.Email);
                if (user != null)
                {
                    if (user.Blocked)
                    {
                        ViewBag.ModalTitle = "Acceso Denegado";
                        ViewBag.ModalMessage = "Su cuenta ha sido bloqueada. Por favor, contacte al administrador.";
                        return View(pUser);
                    }

                    if (Crypto.Compare(pUser.Password, user.Password))
                    {
                        Services.Session.Login(user.Id);
                        Logger.AddLog("User logged in", 1, user.Email);
                        return RedirectToAction("Index", "Home");
                    }
                    else if (user.Role != "webmaster")
                    {
                        Context.Users.SubtractAttemp(user);
                        Verificator.SetVerificator(Context.Users.ListAll().ToList<IEntity>(), "User");

                        if (user.Blocked)
                        {
                            ViewBag.ModalTitle = "Acceso Denegado";
                            ViewBag.ModalMessage = "Su cuenta ha sido bloqueada debido a múltiples intentos fallidos.";
                            return View(pUser);
                        }
                        else
                        {
                            ViewBag.ModalTitle = "Acceso Denegado";
                            ViewBag.ModalMessage = "Contraseña incorrecta.";
                            return View(pUser);
                        }
                    }
                    else
                    {
                        ViewBag.ModalTitle = "Acceso Denegado";
                        ViewBag.ModalMessage = "Contraseña incorrecta.";
                        return View(pUser);
                    }
                }
            }
            return View(pUser);
        }


        // GET: Users/Logout
        [HttpGet]
        public ActionResult Logout()
        {
            if (Services.Session.GetInstance != null)
            {
                Logger.AddLog("User logged out", 1, MyApp.Core.Models.Context.Users.ListAll().Find(x => x.Id == MyApp.Services.Session.GetInstance.id).Email);
                Services.Session.Logout();
                return RedirectToAction("Index", "Home");
            }

            else
            {
                return View();
            }
        }

        // GET: Users/Register
        public ActionResult Register()
        {
            return View();
        }

        // POST: Users/Register
        [HttpPost]
        public ActionResult Register(User pUser)
        {
            try
            {
                if (MyApp.Services.Session.GetInstance == null) return RedirectToAction("Login", "Users");
                Context.Users.Add(pUser);
                Verificator.SetVerificator(Context.Users.ListAll().ToList<IEntity>(), "User");
                Logger.AddLog("User register", 2, pUser.Email);
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Users/ChangePassword
        [HttpGet]
        public ActionResult ChangePassword()
        {
            return View();
        }

        // POST: Users/ChangePassword
        [HttpPost]
        public ActionResult ChangePassword(User pUser)
        {
            try
            {
                var user = Context.Users.ListAll().FirstOrDefault(u => u.Email == pUser.Email);
                if (user == null)
                {
                    ModelState.AddModelError("", "El email ingresado no existe.");
                    return View(pUser);
                }
                else
                {
                    user.Password = Crypto.Parse(pUser.Password);
                    Context.Users.ChangePassword(user);
                    Logger.AddLog("User changed password in", 1, user.Email);
                    Verificator.SetVerificator(Context.Users.ListAll().ToList<IEntity>(), "User");
                    return RedirectToAction("Login", "Users");
                }
            }
            catch
            {
                return View();
            }
        }
    }
}