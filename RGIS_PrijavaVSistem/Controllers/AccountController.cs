using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RGIS_PrijavaVSistem.Services;
using System;

namespace RGIS_PrijavaVSistem.Controllers
{
    public class AccountController : Controller
    {
        private const string SessionKeyUser = "username";

        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost]
        public IActionResult Register(string username, string password)
        {
            try
            {
                UserStore.Register(username, password);
                TempData["msg"] = "Registracija uspešna. Zdaj se lahko prijaviš.";
                return RedirectToAction(nameof(Login));
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View();
            }
        }

        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            var user = UserStore.Login(username, password);
            if (user == null)
            {
                ViewBag.Error = "Napačno uporabniško ime ali geslo.";
                return View();
            }

            HttpContext.Session.SetString(SessionKeyUser, user.Username);
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Remove(SessionKeyUser);
            return RedirectToAction(nameof(Login));
        }
    }
}
