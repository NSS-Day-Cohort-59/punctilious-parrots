using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Claims;
using TabloidMVC.Models;
using TabloidMVC.Models.ViewModels;
using TabloidMVC.Repositories;

namespace TabloidMVC.Controllers

{
    public class CategoreyController : Controller
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoreyController(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }


        // GET: Categorey
        public ActionResult Index()
        {
            var categories = _categoryRepository.GetAll();
            return View(categories);
        }

        // GET: Categorey/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Categorey/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Categorey/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: Categorey/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Categorey/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: Categorey/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Categorey/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
