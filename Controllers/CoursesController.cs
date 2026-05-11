using Controllers;
using DAL;
using Registrar.Models;
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

        [AccessControl.UserAccess(Access.View)]
        public ActionResult GetCourses(bool forceRefresh = false)
        {
            if (forceRefresh || Courses.HasChanged)
            {
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

        // --- MÉTHODE EDIT (GET) CORRIGÉE ---
        [AccessControl.UserAccess(Access.Write)]
        public ActionResult Edit(int id)
        {
            Course course = Courses.Get(id);
            if (course == null) return HttpNotFound();

            // Préparation des listes pour l'outil de sélection des étudiants
            var allStudents = DB.Students.ToList().OrderBy(s => s.LastName).ThenBy(s => s.FirstName).ToList();
            var enrolledStudents = allStudents.Where(s => course.Inscriptions.Contains(s.Id)).ToList();

            ViewBag.Students = SelectListUtilities<Student>.Convert(allStudents);
            ViewBag.Enrolled = SelectListUtilities<Student>.Convert(enrolledStudents);

            return View(course);
        }

        // --- MÉTHODE EDIT (POST) CORRIGÉE ---
        [HttpPost]
        [AccessControl.UserAccess(Access.Write)]
        public ActionResult Edit(Course course, List<int> SelectedStudents) // <-- Ajout de SelectedStudents
        {
            if (ModelState.IsValid)
            {
                // On met à jour les étudiants avec ceux sélectionnés à l'écran
                course.Inscriptions = SelectedStudents ?? new List<int>();
                Courses.Update(course);
                return RedirectToAction("Index");
            }

            // Si le formulaire est invalide (ex: titre vide), on recharge la page
            return RedirectToAction("Edit", new { id = course.Id });
        }

        [AccessControl.UserAccess(Access.Write)]
        public ActionResult Delete(int id)
        {
            Courses.Delete(id);
            return RedirectToAction("Index");
        }
    }
}