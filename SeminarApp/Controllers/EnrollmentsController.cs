using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SeminarApp.Data;
using SeminarApp.Models;

namespace SeminarApp.Controllers
{
    public class EnrollmentsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EnrollmentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        //GET: Enrollments
        public async Task<IActionResult> Index(string sortOrder, string searchString)
        {
            //sorting by po columns - values for view
            ViewData["LastNameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "lastName_desc" : "";
            ViewData["TitleSortParm"] = sortOrder == "Title" ? "title_desc" : "Title";
            ViewData["DateSortParm"] = sortOrder == "Date" ? "date_desc" : "Date";
            ViewData["StatusSortParm"] = sortOrder == "Status" ? "status_desc" : "Status";
            //search
            ViewData["CurrentFilter"] = searchString;

            //load entites
            var enrollments = _context.Enrollments
                .Include(e => e.Course) //eager loading of related data
                .AsNoTracking();

            if (!String.IsNullOrEmpty(searchString))
            {
                enrollments = enrollments.Where(e => e.Course.Title.Contains(searchString)
                                            || e.LastName.Contains(searchString)
                                            || e.FirstName.Contains(searchString)
                                            || e.FirstName.Contains(searchString));
            }

            switch (sortOrder) //sorting by columns
            {
                case "lastName_desc":
                    enrollments = enrollments.OrderByDescending(e => e.LastName);
                    break;
                case "Date":
                    enrollments = enrollments.OrderBy(e => e.Course.StartDate);
                    break;
                case "date_desc":
                    enrollments = enrollments.OrderByDescending(e => e.Course.StartDate);
                    break;
                case "Title":
                    enrollments = enrollments.OrderBy(e => e.Course.Title);
                    break;
                case "title_desc":
                    enrollments = enrollments.OrderByDescending(e => e.Course.Title);
                    break;
                case "Status":
                    enrollments = enrollments.OrderBy(e => e.Status);
                    break;
                case "status_desc":
                    enrollments = enrollments.OrderByDescending(e => e.Status);
                    break;
                default:
                    enrollments = enrollments.OrderBy(e => e.LastName);
                    break;
            }

            return View(await enrollments.ToListAsync());
        }

        //GET: Courses/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Enrollment enrollment = await _context.Enrollments
                .Include(e => e.Course)
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.EnrollmentId == id);

            if (enrollment == null)
            {
                return NotFound();
            }
            return View(enrollment);
        }

        //GET: Courses/Create
        //public IActionResult Create()
        //{
        //    //var course = await _context.Courses.FindAsync(id);
        //    return View();
        //}

        public async Task<IActionResult> Create(int id)
        {
            var course = await _context.Courses.FindAsync(id);
            ViewBag.CourseName = course.Title;
            return View();
        }

        //POST: Courses/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Enrollment enrollment, int id)
        {
            enrollment.CourseId = id;
            if (await TryUpdateModelAsync<Enrollment>(
                enrollment,
                "enrollment",
                e => e.FirstName,
                e => e.LastName,
                e => e.Address,
                e => e.Email,
                e => e.PhoneNumber))
            {
                try
                {
                    _context.Add(enrollment);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Index", "Enroll");
                }
                catch (DbUpdateException /* ex */)
                {
                    ModelState.AddModelError("", "Unable to save changes. " +
                        "Try again, and if the problem persist, " +
                        "see your system administrator.");
                }
            }
            return View(enrollment);
        }

        //GET: Courses/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var enrollment = await _context.Enrollments.FindAsync(id);
            if (enrollment == null)
            {
                return NotFound();
            }

            //za radio button enum - https://dotnettutorials.net/lesson/form-tag-helpers-asp-net-core/
            enrollment.AllStatus = Enum.GetValues(typeof(EnrollmentStatus)).Cast<EnrollmentStatus>().ToList();

            //selectlist Courses
            ViewBag.CourseId = new SelectList(_context.Courses, "CourseId", "Title", enrollment.CourseId);
            return View(enrollment);
        }

        //POST: Courses/Edit/5
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPost(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var enrollmentToUpdate = await _context.Enrollments.FirstOrDefaultAsync(e => e.EnrollmentId == id);
            if (await TryUpdateModelAsync<Enrollment>(
                enrollmentToUpdate,
                "",
                e => e.FirstName,
                e => e.LastName,
                e => e.EnrollmentDate,
                e => e.Address,
                e => e.Email,
                e => e.PhoneNumber,
                e => e.Status,
                e => e.CourseId
                ))
            {
                try
                {
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException /* ex */)
                {
                    ModelState.AddModelError("", "Unable to save changes. " +
                        "Try again, and if the problem persist, " +
                        "see your system administrator.");
                }
            }
            ViewBag.CourseId = new SelectList(_context.Courses, "CourseId", "Title", enrollmentToUpdate.CourseId);
            return View(enrollmentToUpdate);
        }

        //GET: Courses/Delete/5
        public async Task<IActionResult> Delete(int? id, bool? saveChangesError = false)
        {
            if (id == null)
            {
                return NotFound();
            }

            var enrollment = await _context.Enrollments
                .Include(e => e.Course)
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.EnrollmentId == id);
            if (enrollment == null)
            {
                return NotFound();
            }

            if (saveChangesError.GetValueOrDefault())
            {
                ViewData["ErrorMessage"] =
                    "Delete failed. Try again, and if the problem persists " +
                    "see your system administrator.";
            }
            return View(enrollment);
        }

        //POST: Courses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var enrollment = await _context.Enrollments.FindAsync(id);

            if (enrollment == null)
            {
                return RedirectToAction(nameof(Index));
            }

            try
            {
                _context.Enrollments.Remove(enrollment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException /* ex */)
            {
                //Log the error (uncomment ex variable name and write a log.)
                return RedirectToAction(nameof(Delete), new { id = id, saveChangesError = true });
            }
        }
    }
}