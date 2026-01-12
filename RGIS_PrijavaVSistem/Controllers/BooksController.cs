using Microsoft.AspNetCore.Mvc;
using RGIS_PrijavaVSistem.Models;
using RGIS_PrijavaVSistem.Services;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace RGIS_PrijavaVSistem.Controllers
{
    public class BooksController : Controller
    {
        private bool IsLoggedIn()
        {
            return HttpContext.Session.GetString("username") != null;
        }
        public IActionResult Index(string search)
        {
            List<Book> books = string.IsNullOrWhiteSpace(search)
                ? BookRepository.GetAll()
                : BookRepository.Search(search);

            return View(books);
        }

        public IActionResult Create()
        {
            if (!IsLoggedIn())
                return RedirectToAction("Login", "Account");

            return View();
        }

        [HttpPost]
        public IActionResult Create(Book book)
        {
            if (!IsLoggedIn())
                return RedirectToAction("Login", "Account");
            if (string.IsNullOrWhiteSpace(book.Title) ||
                string.IsNullOrWhiteSpace(book.Author))
            {
                ModelState.AddModelError("", "Naslov in avtor sta obvezna.");
                return View(book);
            }

            BookRepository.Add(book);
            return RedirectToAction(nameof(Index));
        }
    }
}
