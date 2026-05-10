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
        public ActionResult Edit(Student student, List<int> SelectedCourses) // <-- Changé pour List<int>
        {
            if (ModelState.IsValid)
            {
                Students.Update(student);

                // Mise à jour des inscriptions
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
            return View(student);
        }
    }
}