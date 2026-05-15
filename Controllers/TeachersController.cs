using Controllers;
using DAL;
using Registrar.Models;
using System;
using System.Collections.Generic;
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

        [AccessControl.UserAccess(Access.View)]
        public ActionResult GetTeacherInfo(int id, bool forceRefresh = false)
        {
            if (forceRefresh || DB.Teachers.HasChanged)
            {
                var teacher = DB.Teachers.Get(id);
                if (teacher == null) return HttpNotFound();
                return PartialView("_TeacherInfo", teacher);
            }
            return null;
        }

        [AccessControl.UserAccess(Access.View)]
        public ActionResult GetTeacherAllocations(int id, bool forceRefresh = false)
        {
            // On rafraîchit si le prof OU les cours (allocations) ont changé
            if (forceRefresh || DB.Teachers.HasChanged || DB.Courses.HasChanged)
            {
                var teacher = DB.Teachers.Get(id);
                if (teacher == null) return null;

                var allocationsList = new List<Tuple<int, Course>>();
                foreach (var c in DB.Courses.ToList())
                {
                    foreach (var a in c.Allocations)
                    {
                        if (a.Value == id)
                            allocationsList.Add(new Tuple<int, Course>(a.Key, c));
                    }
                }

                ViewBag.GroupedAllocations = allocationsList
                    .GroupBy(x => x.Item1)
                    .OrderByDescending(g => g.Key)
                    .ToList();

                return PartialView("_TeacherAllocations", teacher);
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


        [HttpPost]
        [AccessControl.UserAccess(Access.Write)]
        public ActionResult Edit(Teacher teacher, List<int> SelectedCourses)
        {
            if (ModelState.IsValid)
            {
                Teachers.Update(teacher);

                int currentYear = Registrar.Models.NextSession.Year;
                var validSessions = Registrar.Models.NextSession.ValidSessions;

                var allCourses = DAL.DB.Courses.ToList().ToArray();

                foreach (var course in allCourses)
                {
                    // On ne traite que les cours de la session courante (Hiver ou Automne)
                    if (!validSessions.Contains(course.Session)) continue;

                    bool shouldBeAssigned = SelectedCourses != null && SelectedCourses.Contains(course.Id);
                    bool isCurrentlyAssigned = course.Allocations.ContainsKey(currentYear) && course.Allocations[currentYear] == teacher.Id;

                    if (shouldBeAssigned && !isCurrentlyAssigned)
                    {
                        // On ajoute le prof au cours
                        course.Allocations[currentYear] = teacher.Id;
                        DAL.DB.Courses.Update(course); // Ne plantera plus !
                    }
                    else if (!shouldBeAssigned && isCurrentlyAssigned)
                    {
                        // On retire le prof du cours car il a été désélectionné
                        course.Allocations.Remove(currentYear);
                        DAL.DB.Courses.Update(course); 
                    }
                }

                return RedirectToAction("Index");
            }

            int year = Registrar.Models.NextSession.Year;
            var courses = DAL.DB.Courses.ToList().Where(c => Registrar.Models.NextSession.ValidSessions.Contains(c.Session)).OrderBy(c => c.Code).ToList();
            var assigned = courses.Where(c => c.Allocations.ContainsKey(year) && c.Allocations[year] == teacher.Id).ToList();
            ViewBag.Courses = SelectListUtilities<Course>.Convert(courses);
            ViewBag.Allocations = SelectListUtilities<Course>.Convert(assigned);

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
            Teacher teacher = DAL.DB.Teachers.Get(id);
            if (teacher == null) return HttpNotFound();

            ViewBag.Title = "Professeur - Détails";

            // 1. On force la création d'une liste explicite de Tuples pour éviter le type anonyme
            var allocationsList = new List<Tuple<int, Course>>();

            foreach (var c in DAL.DB.Courses.ToList())
            {
                foreach (var a in c.Allocations)
                {
                    if (a.Value == id)
                    {
                        // Ajout explicite d'un Tuple <Année, Cours>
                        allocationsList.Add(new Tuple<int, Course>(a.Key, c));
                    }
                }
            }

            // 2. On groupe cette liste sécurisée
            ViewBag.GroupedAllocations = allocationsList
                .GroupBy(x => x.Item1)
                .OrderByDescending(g => g.Key)
                .ToList();

            return View(teacher);
        }

        [AccessControl.UserAccess(Access.Write)]
        public ActionResult Edit(int id)
        {
            Teacher teacher = Teachers.Get(id);
            if (teacher == null) return HttpNotFound();

            ViewBag.Title = "Professeur - Modification";

            // On prépare la double liste pour la session courante uniquement
            int currentYear = Registrar.Models.NextSession.Year;
            var allCourses = DAL.DB.Courses.ToList()
                .Where(c => Registrar.Models.NextSession.ValidSessions.Contains(c.Session))
                .OrderBy(c => c.Code).ToList();

            var assignedCourses = allCourses
                .Where(c => c.Allocations.ContainsKey(currentYear) && c.Allocations[currentYear] == id)
                .ToList();

            ViewBag.Courses = SelectListUtilities<Course>.Convert(allCourses);
            ViewBag.Allocations = SelectListUtilities<Course>.Convert(assignedCourses);

            return View(teacher);
        }
    }
}