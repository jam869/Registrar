using Controllers;
using DAL;
using Registrar.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Registrar.Controllers
{
    public class CoursesController : Controller
    {
        private DAL.Repository<Course> Courses => DB.Courses;

        [AccessControl.UserAccess(Access.View)]
        public ActionResult Index()
        {
            return View();
        }

        // Action pour le rafraîchissement AJAX (appelée toutes les 5 sec)
        [AccessControl.UserAccess(Access.View)]
        public ActionResult GetCourses(bool forceRefresh = false)
        {
            if (forceRefresh || Courses.HasChanged)
            {
                // On trie par session puis par sigle
                var list = Courses.ToList().OrderBy(c => c.Session).ThenBy(c => c.Code).ToList();
                return PartialView(list);
            }
            return null;
        }

        [AccessControl.UserAccess(Access.Write)]
        public ActionResult Create()
        {
            return View(new Course());
        }

        [HttpPost]
        [AccessControl.UserAccess(Access.Write)]
        public ActionResult Create(Course course)
        {
            if (ModelState.IsValid)
            {
                Courses.Add(course);
                return RedirectToAction("Index");
            }
            return View(course);
        }

        [AccessControl.UserAccess(Access.Write)]
        public ActionResult Edit(int id)
        {
            Course course = Courses.Get(id);
            if (course == null) return HttpNotFound();
            return View(course);
        }

        [HttpPost]
        [AccessControl.UserAccess(Access.Write)]
        public ActionResult Edit(Course course)
        {
            if (ModelState.IsValid)
            {
                Courses.Update(course);
                return RedirectToAction("Index");
            }
            return View(course);
        }

        [AccessControl.UserAccess(Access.Write)]
        public ActionResult Delete(int id)
        {
            Courses.Delete(id);
            return RedirectToAction("Index");
        }
    }
}