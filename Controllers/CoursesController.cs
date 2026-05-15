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

        [AccessControl.UserAccess(Access.View)]
        public ActionResult GetCourseInfo(int id, bool forceRefresh = false)
        {
            if (forceRefresh || DB.Courses.HasChanged)
            {
                var course = DB.Courses.Get(id);
                if (course == null) return null;
                return PartialView("_CourseInfo", course);
            }
            return null;
        }

        [AccessControl.UserAccess(Access.View)]
        public ActionResult GetCourseStudents(int id, bool forceRefresh = false)
        {
            if (forceRefresh || DB.Courses.HasChanged || DB.Students.HasChanged || DB.Teachers.HasChanged)
            {
                var course = DB.Courses.Get(id);
                if (course == null) return null;

                var enrolledStudents = DB.Students.ToList()
                    .Where(s => course.Inscriptions.Contains(s.Id))
                    .OrderBy(s => s.LastName)
                    .ToList();

                ViewBag.GroupedInscriptions = enrolledStudents
                    .GroupBy(s => s.Year)
                    .OrderByDescending(g => g.Key)
                    .ToList();

                return PartialView("_CourseStudents", course);
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

        [AccessControl.UserAccess(Access.View)]
        public ActionResult Details(int id)
        {
            Course course = Courses.Get(id);
            if (course == null) return HttpNotFound();

            ViewBag.Title = "Cours - Détails";

            var enrolledStudents = DB.Students.ToList()
                .Where(s => course.Inscriptions.Contains(s.Id))
                .ToList();

            ViewBag.GroupedInscriptions = enrolledStudents
                .GroupBy(s => s.Year)
                .OrderByDescending(g => g.Key)
                .ToList();

            return View(course);
        }

        [AccessControl.UserAccess(Access.Write)]
        public ActionResult Edit(int id)
        {
            Course course = Courses.Get(id);
            if (course == null) return HttpNotFound();

            ViewBag.Title = "Cours - Modification";

            var allStudents = DB.Students.ToList().OrderBy(s => s.LastName).ThenBy(s => s.FirstName).ToList();
            var enrolledStudents = allStudents.Where(s => course.Inscriptions.Contains(s.Id)).ToList();

            ViewBag.Students = SelectListUtilities<Student>.Convert(allStudents);
            ViewBag.Enrolled = SelectListUtilities<Student>.Convert(enrolledStudents);

            return View(course);
        }

        [HttpPost]
        [AccessControl.UserAccess(Access.Write)]
        public ActionResult Edit(Course course, List<int> SelectedStudents) 
        {
            if (ModelState.IsValid)
            {
                course.Inscriptions = SelectedStudents ?? new List<int>();
                Courses.Update(course);
                return RedirectToAction("Index");
            }

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