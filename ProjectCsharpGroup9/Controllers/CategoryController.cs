﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProjectCsharpGroup9.Models;

namespace ProjectCsharpGroup9.Controllers
{
    public class CategoryController : Controller
    {
        // GET: CategoryController
        AppDbContext _dbContext;

        public CategoryController()
        {
            _dbContext = new AppDbContext();
        }

        public ActionResult Index()
        {
            var GetAll = _dbContext.Categories.ToList();
            return View(GetAll);
        }

        public ActionResult Details(Guid Id)
        {
            var GetDetails = _dbContext.Categories.Find(Id);
            return View(GetDetails);
        }

        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Category category)
        {
            try
            {
                _dbContext.Categories.Add(category);
                _dbContext.SaveChanges();
                return RedirectToAction("Index");
            }
            catch
            {
                return BadRequest();
            }
        }

        public ActionResult Edit(Guid Id)
        {
            var GetEdit = _dbContext.Categories.Find(Id);
            return View(GetEdit);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Category category)
        {
            try
            {
                var GetEdit = _dbContext.Categories.Find(category.CategoryID);
                GetEdit.Name = category.Name;
             GetEdit.Description = category.Description;
                _dbContext.Categories.Update(GetEdit);
                _dbContext.SaveChanges();
                return RedirectToAction("Index");
            }
            catch
            {
                return BadRequest();
            }
        }

        public ActionResult Delete(Guid Id)
        {
            try
            {
                var GetDelete = _dbContext.Categories.Find(Id);
                _dbContext.Categories.Remove(GetDelete);
                _dbContext.SaveChanges();
                return RedirectToAction("Index");
            }
            catch
            {
                return BadRequest();
            }
        }
    }
}
