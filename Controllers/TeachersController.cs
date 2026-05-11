using Controllers;
using Registrar.Models;
using System;
using System.Linq;
using System.Web.Mvc;

namespace Registrar.Controllers
{
    public class TeachersController : Controller
    {
        private DAL.Repository<Teacher> Teachers => DAL.DB.Teachers;

        [AccessControl.UserAccess(Access.View)]
        public ActionResult Index()
        {
            return View();
        }

        // Action pour l'auto-refresh
        [AccessControl.UserAccess(Access.View)]
        public ActionResult GetTeachers(bool forceRefresh = false)
        {
            if (forceRefresh || Teachers.HasChanged)
            {
                var list = Teachers.ToList().OrderBy(t => t.LastName).ThenBy(t => t.FirstName).ToList();
                return PartialView(list);
            }
            return null;
        }

        [AccessControl.UserAccess(Access.Write)]
        public ActionResult Create()
        {
            return View(new Teacher());
        }

        [HttpPost]
        [AccessControl.UserAccess(Access.Write)]
        public ActionResult Create(Teacher teacher)
        {
            // On retire le code de la validation car il sera généré ici
            ModelState.Remove("Code");

            if (ModelState.IsValid)
            {
                // Génération automatique : CLG-420- + 5 chiffres aléatoires
                Random rnd = new Random();
                teacher.Code = "CLG-420-" + rnd.Next(10000, 99999).ToString();

                Teachers.Add(teacher);
                return RedirectToAction("Index");
            }
            return View(teacher);
        }

        [AccessControl.UserAccess(Access.Write)]
        public ActionResult Edit(int id)
        {
            Teacher teacher = Teachers.Get(id);
            if (teacher == null) return HttpNotFound();
            return View(teacher);
        }

        [HttpPost]
        [AccessControl.UserAccess(Access.Write)]
        public ActionResult Edit(Teacher teacher)
        {
            if (ModelState.IsValid)
            {
                Teachers.Update(teacher);
                return RedirectToAction("Index");
            }
            return View(teacher);
        }

        [AccessControl.UserAccess(Access.Write)]
        public ActionResult Delete(int id)
        {
            Teachers.Delete(id);
            return RedirectToAction("Index");
        }

        [AccessControl.UserAccess(Access.View)]
        public ActionResult Details(int id)
        {
            Teacher teacher = Teachers.Get(id);
            if (teacher == null) return HttpNotFound();
            return View(teacher);
        }
    }
}