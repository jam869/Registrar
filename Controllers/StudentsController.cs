using Controllers;
using DAL;
using Registrar.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Registrar.Controllers
{
    [AccessControl.UserAccess(Access.View)]
    public class StudentsController : Controller
    {
        private DAL.Repository<Student> Students => DB.Students;
        private DAL.Repository<Course> Courses => DB.Courses;

        // GET: Students/Edit/5
        // GET: Students/Edit/5
        [AccessControl.UserAccess(Access.Write)]
        public ActionResult Edit(int id)
        {
            Student student = Students.Get(id);
            if (student == null) return HttpNotFound();

            var validSessions = NextSession.ValidSessions;
            var allCourses = Courses.ToList().Where(c => validSessions.Contains(c.Session)).OrderBy(c => c.Code).ToList();

            // On trouve les cours de l'étudiant
            var registeredCourses = allCourses.Where(c => c.Inscriptions.Contains(id)).ToList();

            // UTILISATION DE SELECTLISTUTILITIES (Obligatoire pour l'outil du prof)
            ViewBag.Courses = SelectListUtilities<Course>.Convert(allCourses);
            ViewBag.Registrations = SelectListUtilities<Course>.Convert(registeredCourses);

            return View(student);
        }

        [AccessControl.UserAccess(Access.View)]
        public ActionResult GetStudentInfo(int id, bool forceRefresh = false)
        {
            // Rafraîchit si un changement a eu lieu dans les étudiants
            if (forceRefresh || Students.HasChanged)
            {
                var student = Students.Get(id);
                if (student == null) return null;
                return PartialView("_StudentInfo", student);
            }
            return null;
        }

        [AccessControl.UserAccess(Access.View)]
        public ActionResult GetStudentInscriptions(int id, bool forceRefresh = false)
        {
            // Rafraîchit si un changement a eu lieu dans les étudiants OU les cours
            if (forceRefresh || Students.HasChanged || Courses.HasChanged)
            {
                var student = Students.Get(id);
                if (student == null) return null;

                var courses = Courses.ToList().Where(c => c.Inscriptions.Contains(id)).ToList();

                // Le calcul des années
                ViewBag.GroupedInscriptions = courses
                    .GroupBy(c => student.Year + ((c.Session - 1) / 2))
                    .OrderByDescending(g => g.Key)
                    .ToList();

                return PartialView("_StudentInscriptions", student);
            }
            return null;
        }
        // POST: Students/Edit/5
        [HttpPost]
        [AccessControl.UserAccess(Access.Write)]
        public ActionResult Edit(Student student, List<int> SelectedCourses)
        {
            if (ModelState.IsValid)
            {
                Students.Update(student);

                // Mise à jour de la relation dans les cours
                var validSessions = NextSession.ValidSessions;
                var currentSessionCourses = Courses.ToList().Where(c => validSessions.Contains(c.Session)).ToList();

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

            var allCoursesValid = Courses.ToList().Where(c => NextSession.ValidSessions.Contains(c.Session)).OrderBy(c => c.Code).ToList();
            ViewBag.Courses = SelectListUtilities<Course>.Convert(allCoursesValid);
            ViewBag.Registrations = SelectListUtilities<Course>.Convert(allCoursesValid.Where(c => c.Inscriptions.Contains(student.Id)).ToList());

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
        [AccessControl.UserAccess(Access.View)]
        public ActionResult GetInscriptions(int id, bool forceRefresh = false)
        {
            if (forceRefresh || DB.Courses.HasChanged)
            {
                var student = Students.Get(id);
                var validSessions = NextSession.ValidSessions;

                ViewBag.CurrentCourses = DB.Courses.ToList()
                    .Where(c => c.Inscriptions.Contains(id) && validSessions.Contains(c.Session))
                    .OrderBy(c => c.Code).ToList();

                ViewBag.AvailableCourses = DB.Courses.ToList()
                    .Where(c => !c.Inscriptions.Contains(id) && validSessions.Contains(c.Session))
                    .OrderBy(c => c.Code).ToList();

                return PartialView("_Inscriptions");
            }
            return null;
        }

        [AccessControl.UserAccess(Access.Write)]
        public ActionResult Create()
        {
            return View(new Student());
        }

      
        [HttpPost]
        [AccessControl.UserAccess(Access.Write)]
        public ActionResult Create(Student student)
        {
            // On retire le code des erreurs de validation puisqu'on va le générer
            ModelState.Remove("Code");

            if (ModelState.IsValid)
            {
                // Génération automatique du code (Année + 6 chiffres aléatoires)
                Random rnd = new Random();
                student.Code = DateTime.Now.Year.ToString() + rnd.Next(100000, 999999).ToString();

                Students.Add(student);
                return RedirectToAction("Index");
            }
            return View(student);
        }

        [AccessControl.UserAccess(Access.View)]
        public ActionResult Details(int id)
        {
            Student student = Students.Get(id);
            if (student == null) return HttpNotFound();

            ViewBag.Title = "Étudiants - Détails";

            var courses = Courses.ToList().Where(c => c.Inscriptions.Contains(id)).ToList();

            // Calcul exact : (Session 1 et 2 = +0 an), (Session 3 et 4 = +1 an), etc.
            ViewBag.GroupedInscriptions = courses
                .GroupBy(c => student.Year + ((c.Session - 1) / 2))
                .OrderByDescending(g => g.Key)
                .ToList();

            return View(student);
        }
    }
}