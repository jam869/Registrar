using Controllers;
using Registrar.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Registrar.Controllers
{
    [AccessControl.UserAccess(Access.View)]
    public class StudentsController : Controller
    {
        private StudentsRepository Students = new StudentsRepository();
        private CoursesRepository Courses = new CoursesRepository();

        // GET: Students/Edit/5
        [AccessControl.UserAccess(Access.Write)]
        public ActionResult Edit(int id) // <-- Changé pour int
        {
            Student student = Students.Get(id);
            if (student == null) return HttpNotFound();

            // Filtrer les cours selon la session courante définie par le prof
            var validSessions = NextSession.ValidSessions;

            ViewBag.CurrentCourses = Courses.ToList()
                .Where(c => c.Inscriptions.Contains(id) && validSessions.Contains(c.Session))
                .OrderBy(c => c.Sigle).ToList();

            ViewBag.AvailableCourses = Courses.ToList()
                .Where(c => !c.Inscriptions.Contains(id) && validSessions.Contains(c.Session))
                .OrderBy(c => c.Sigle).ToList();

            return View(student);
        }
        [HttpPost]
        [AccessControl.UserAccess(Access.Write)]
        public ActionResult Edit(Student student, List<int> SelectedCourses)
        {
            if (ModelState.IsValid)
            {
                Students.Update(student);

                // Mise à jour des inscriptions
                var currentSessions = NextSession.ValidSessions;
                var currentSessionCourses = Courses.ToList().Where(c => currentSessions.Contains(c.Session)).ToList();

                foreach (var course in currentSessionCourses)
                {
                    bool wasEnrolled = course.Inscriptions.Contains(student.Id);
                    bool shouldBeEnrolled = SelectedCourses != null && SelectedCourses.Contains(course.Id);

                    if (shouldBeEnrolled && !wasEnrolled)
                    {
                        course.Inscriptions.Add(student.Id);
                        Courses.Update(course);
                    }
                    else if (!shouldBeEnrolled && wasEnrolled)
                    {
                        course.Inscriptions.Remove(student.Id);
                        Courses.Update(course);
                    }
                }
                return RedirectToAction("Index");
            }

            // --- AJOUTER CECI ICI ---
            // Si on arrive ici, c'est que le formulaire est invalide. 
            // On doit impérativement recharger les listes avant de réafficher la page !
            var validSessions = NextSession.ValidSessions;

            ViewBag.CurrentCourses = Courses.ToList()
                .Where(c => c.Inscriptions.Contains(student.Id) && validSessions.Contains(c.Session))
                .OrderBy(c => c.Sigle).ToList();

            ViewBag.AvailableCourses = Courses.ToList()
                .Where(c => !c.Inscriptions.Contains(student.Id) && validSessions.Contains(c.Session))
                .OrderBy(c => c.Sigle).ToList();
            // ------------------------

            return View(student);
        }
        // GET: Students
        [AccessControl.UserAccess(Access.View)]
        public ActionResult Index()
        {
            // On récupère la liste des étudiants pour l'afficher
            return View(Students.ToList().OrderBy(s => s.LastName).ToList());
        }
        // Cette action sera appelée en boucle par JavaScript (AJAX)
        [AccessControl.UserAccess(Access.View)]
        public ActionResult GetStudents(bool forceRefresh = false)
        {
            // On vérifie si la DB a changé ou si c'est le premier chargement
            if (forceRefresh || Students.HasChanged)
            {
                return PartialView(Students.ToList().OrderBy(s => s.LastName).ToList());
            }
            return null; // Si rien n'a changé, on ne renvoie rien (économise la bande passante)
        }
    }
}