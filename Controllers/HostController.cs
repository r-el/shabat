﻿using Microsoft.AspNetCore.Mvc;
using shabat2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data.Entity;
using shabat2.ViewModel;

namespace shabat2.Controllers
{
    public class HostController : Controller
    {
        public IActionResult Index()
        {
            // טען מהדטאבייס את כל הקטגוריות עם המאכלים
            List<Category> categories = DAL.Get.Categories.Include(c=>c.Foods).ToList();
            // טען מהדטאבייס את כל האורחים
            List<Guest> guests = DAL.Get.Guests.Include(g=>g.Foods).ToList();
            VMHostMain vm = new VMHostMain{ Categories = categories, Guests = guests };
            return View(vm);
        }

        public IActionResult CategoryDetails(int? id)
        {// פרטי קבוצה
            if (id == null) return RedirectToAction(nameof(Index)); // ודא קבלת ערך

            // טען מהדטאבייס את הקטגוריה עם המאכלים שלו
            Category category = DAL.Get.Categories.Include(c => c.Foods).ToList().Find(c => c.ID == id);
            if (category == null) return RedirectToAction(nameof(Index)); // ודא קבלת ערך
            return View(category);
        }

        public IActionResult FoodDetails(int? id)
        {// פרטי מאכל
            if (id == null) return RedirectToAction(nameof(Index)); // ודא קבלת ערך

            // טען מהדטאבייס את המאכל עם הקטגוריה שלו
            Food food = DAL.Get.Foods.Include(f=>f.Category).ToList().Find(f => f.ID == id);
            if (food == null) return RedirectToAction(nameof(Index)); // ודא קבלת ערך
            return View(food);
        }

        public IActionResult AddCategory()
        {// הוספת קבוצה
            VMAddCategory vm = new VMAddCategory { Category = new Category() };
            return View(vm);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddCategory(VMAddCategory vm)
        {
            if (vm.Food != string.Empty) vm.Category.AddFood(vm.Food, vm.FoodFile);
            vm.Category.SetPhoto = vm.File;
            DAL.Get.Categories.Add(vm.Category);
            // שמירת הנתונים בדטאבייס
            DAL.Get.SaveChanges();
            return View("CategoryDetails", vm.Category);
        }

        public IActionResult AddFood(int? id)
        {// הוספת מאכל
            if (id == null) return RedirectToAction(nameof(Index)); // ודא קבלת ערך
            List<Category> categories = DAL.Get.Categories.Include(c => c.Foods).ToList();
            Category category = categories.Find(c => c.ID == id);
            if (category == null) return RedirectToAction(nameof(Index)); // ודא קבלת ערך
            // השמת נתונים
            VMAddFood vm = new VMAddFood
            {
                Categories = categories,
                Category = category,
                CategoryId = category.ID,
            };
            return View(vm);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddFood(VMAddFood vm)
        {
            DAL.Get.Categories.ToList().Find(c => c.ID == vm.CategoryId).AddFood(vm.Food, vm.File);
            // שמירת הנתונים בדטאבייס
            DAL.Get.SaveChanges();
            return RedirectToAction(nameof(CategoryDetails), new { id = vm.CategoryId });
        }

        public IActionResult Edit(int? id)
        { // עריכת קטגוריה
            if (id == null) return RedirectToAction(nameof(Index)); // ודא קבלת ערך
            // טען מהדטאבייס את הקטגוריה
            Category category = DAL.Get.Categories.ToList().Find(c => c.ID == id);
            if (category == null) return RedirectToAction(nameof(Index)); // ודא קבלת ערך
            // השמת נתונים
            VMEditCategory vm = new VMEditCategory
            {
                Category = category,
                CategoryID = category.ID
            };
            return View(vm);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(VMEditCategory vM)
        {
            // טען מהדטאבייס את הקטגוריה עם המאכלים
            Category category = DAL.Get.Categories.Include(c=>c.Foods).ToList().Find(c => c.ID == vM.CategoryID);
            // השמת נתונים
            category.CategoryName = vM.Category.CategoryName;
            category.SetPhoto = vM.File;
            // שמירת נתונים בדטאבייס
            DAL.Get.SaveChanges();
            return View("CategoryDetails", category);
        }

        public IActionResult EditFood(int? id)
        {// עריכת מאכל
            if (id == null) return RedirectToAction(nameof(Index)); // ודא קבלת ערך
            // טען מהדטאבייס את הקטגוריה
            Food food= DAL.Get.Foods.ToList().Find(f => f.ID == id);
            if (food == null) return RedirectToAction(nameof(Index)); // ודא קבלת ערך
            // השמת נתונים
            VMEditFood vM = new VMEditFood { Food = food, FoodID = food.ID };
            return View(vM);
        }
    }
}
